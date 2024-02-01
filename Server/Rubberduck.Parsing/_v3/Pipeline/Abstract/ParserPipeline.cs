using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using System.Runtime.CompilerServices;
using System.Threading.Tasks.Dataflow;

namespace Rubberduck.Parsing._v3.Pipeline.Abstract;

public interface IParserPipeline
{
    /// <summary>
    /// Starts executing the parser pipeline for the provided input.
    /// </summary>
    Task StartAsync(object input);
    /// <summary>
    /// Cancels the ongoing execution of the pipeline.
    /// </summary>
    /// <remarks>
    /// If a block does not collaboratively cancel, execution stops when the currently executing block exits.
    /// </remarks>
    void Cancel();

    /// <summary>
    /// Gets a <c>Task</c> that completes when the last pipeline block has completed, regardless of its state.
    /// </summary>
    Task Completion { get; }
}
public interface IParserPipeline<TInput> : IParserPipeline
{
    /// <summary>
    /// Starts executing the parser pipeline for the provided input.
    /// </summary>
    Task StartAsync(TInput input, CancellationTokenSource? tokenSource = null);
}

public abstract class ParserPipeline<TInput, TState> : ServiceBase, IParserPipeline<TInput>, IDisposable
{
    protected static DataflowLinkOptions WithCompletionPropagation { get; } = new() { PropagateCompletion = true };
    protected static DataflowLinkOptions WithoutCompletionPropagation { get; } = new() { PropagateCompletion = false };

    protected GroupingDataflowBlockOptions GreedyJoinExecutionOptions { get; }
    protected ExecutionDataflowBlockOptions ExecutionOptions { get; }

    protected CancellationTokenSource TokenSource { get; private set; } = new();

    private readonly List<IDisposable> _disposables = [];

    protected ParserPipeline(ILogger<WorkspaceParserPipeline> logger,
        RubberduckSettingsProvider settingsProvider,
        PerformanceRecordAggregator performance)
        : base(logger, settingsProvider, performance)
    {
        ExecutionOptions = new() 
        { 
            CancellationToken = TokenSource.Token,
            MaxDegreeOfParallelism = 4, // TODO make this configurable
        };
        GreedyJoinExecutionOptions = new()
        {
            CancellationToken = TokenSource.Token,
            Greedy = true,
        };

        (InputBlock, Completion) = DefinePipelineBlocks();
    }

    protected CancellationToken Token => TokenSource.Token;
    public void Cancel() => TokenSource.Cancel();

    private TInput InputParameter { get; set; } = default!;
    public TState State { get; protected set; } = default!;

    protected virtual void SetInitialState(TInput input) => State = default!;

    /// <summary>
    /// Defines and links all pipeline blocks.
    /// </summary>
    /// <returns>
    /// Returns the input block and completion task of the pipeline.
    /// </returns>
    protected abstract (ITargetBlock<TInput> inputBlock, Task completion) DefinePipelineBlocks();

    /// <summary>
    /// Gets the <c>IDataflowBlock</c> that receives the input when the pipeline is started.
    /// </summary>
    /// <remarks>
    /// Useful for linking different pipelines.
    /// </remarks>
    public IDataflowBlock InputBlock { get; }

    public Task Completion { get; }

    public virtual Task StartAsync(object input) => StartAsync((TInput)input, TokenSource);

    /// <summary>
    /// Starts the pipeline by posting the specified input to the <c>InputBlock</c>.
    /// </summary>
    /// <remarks>
    /// Returns immediately with the <c>Completion</c> task.
    /// </remarks>
    public virtual Task StartAsync(TInput input, CancellationTokenSource? tokenSource = null)
    {
        if (State != null)
        {
            throw new InvalidOperationException("BUG: This pipeline instance is already in use; a new one should be created every time.");
        }

        if (tokenSource != null)
        {
            TokenSource = tokenSource;
        }

        InputParameter = input;
        SetInitialState(input);

        ((ITargetBlock<TInput>)InputBlock).Post(input);
        InputBlock.Complete();

        return Completion;
    }

    private void LogBlockActionStart(string? actionName)
    {
        LogTrace($"Thread {Environment.CurrentManagedThreadId} is starting pipeline action '{GetType().Name}::{actionName ?? "(null)"}'.");
    }

    private void LogBlockActionEnd(string? actionName)
    {
        LogTrace($"Thread {Environment.CurrentManagedThreadId} has completed pipeline action '{GetType().Name}::{actionName ?? "(null)"}'.");
    }

    protected virtual TResult RunTransformBlock<T, TResult>(IDataflowBlock block, T param, Func<T, TResult> action, [CallerMemberName]string? actionName = null) where TResult : class
    {
        LogBlockActionStart(actionName);
        TResult? result = null;
        if (State != null && !TryRunAction(() =>
        {
            ThrowIfCancellationRequested();
            result = action.Invoke(param);
            ThrowIfCancellationRequested();

        }, out var exception, actionName) && exception != null)
        {
            FaultDataflowBlock(block, exception);
        }

        LogBlockActionEnd(actionName);
        return result ?? throw new InvalidOperationException("Result was unexpectedly null.");
    }

    protected virtual TResult RunTransformBlock<T, TResult>(IDataflowBlock block, T param, Func<T, CancellationToken, TResult> action, [CallerMemberName] string? actionName = null) where TResult : class
    {
        LogBlockActionStart(actionName);
        TResult? result = null;
        if (State != null && !TryRunAction(() =>
        {
            ThrowIfCancellationRequested();
            result = action.Invoke(param, Token);
            ThrowIfCancellationRequested();

        }, out var exception, actionName) && exception != null)
        {
            FaultDataflowBlock(block, exception);
        }

        LogBlockActionEnd(actionName);
        return result ?? throw new InvalidOperationException("Result was unexpectedly null.");
    }

    protected virtual void RunActionBlock<T>(IDataflowBlock block, T param, Action<T> action, [CallerMemberName] string? actionName = null)
    {
        LogBlockActionStart(actionName);

        if (State != null && !TryRunAction(() =>
        {
            ThrowIfCancellationRequested();
            action.Invoke(param);
            ThrowIfCancellationRequested();

        }, out var exception, actionName) && exception != null)
        {
            FaultDataflowBlock(block, exception);
        }

        LogBlockActionEnd(actionName);
    }

    protected virtual void RunActionBlock<T>(IDataflowBlock block, T param, Action<T, CancellationToken> action, [CallerMemberName] string? actionName = null)
    {
        LogBlockActionStart(actionName);

        if (State != null && !TryRunAction(() =>
        {
            ThrowIfCancellationRequested();
            action.Invoke(param, Token);
            ThrowIfCancellationRequested();

        }, out var exception, actionName) && exception != null)
        {
            FaultDataflowBlock(block, exception);
        }

        LogBlockActionEnd(actionName);
    }

    protected void Link<T>(ISourceBlock<T> source, ITargetBlock<T> target, DataflowLinkOptions options)
    {
        _disposables.Add(source.LinkTo(target, options));
    }

    protected void FaultDataflowBlock(IDataflowBlock block, Exception exception)
    {
        block.Fault(exception);
        TokenSource?.Cancel();
    }

    protected void ThrowIfCancellationRequested() => TokenSource.Token.ThrowIfCancellationRequested();

    public void Dispose()
    {
        TokenSource?.Dispose();
        foreach (var disposable in _disposables)
        {
            disposable.Dispose();
        }

        GC.SuppressFinalize(this);
    }
}
