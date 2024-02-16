using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using System;
using System.Collections.Immutable;
using System.Reactive.Disposables;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks.Dataflow;
using System.Windows.Forms.Design;
using System.Windows.Media.Animation;

namespace Rubberduck.Parsing._v3.Pipeline.Abstract;

public interface IParserPipeline
{
    /// <summary>
    /// Starts executing the parser pipeline for the provided input.
    /// </summary>
    Task StartAsync(object input, object state, CancellationTokenSource? tokenSource);
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
public interface IParserPipeline<TInput, TState> : IParserPipeline
{
    /// <summary>
    /// Starts executing the parser pipeline for the provided input.
    /// </summary>
    Task StartAsync(TInput input, TState state, CancellationTokenSource? tokenSource);
}

public abstract class DataflowPipeline : ServiceBase, IDisposable
{
    private readonly List<IDisposable> _links = [];
    protected static DataflowLinkOptions WithCompletionPropagation { get; } = new() { PropagateCompletion = true };
    protected static DataflowLinkOptions WithoutCompletionPropagation { get; } = new() { PropagateCompletion = false };


    protected DataflowPipeline(ILogger logger, RubberduckSettingsProvider settingsProvider, PerformanceRecordAggregator performance) 
        : base(logger, settingsProvider, performance)
    {
        DataflowBlocks = DefinePipelineSections().ToImmutableArray();
    }

    protected virtual CancellationTokenSource? TokenSource { get; set; }
    protected CancellationToken Token => TokenSource?.Token ?? CancellationToken.None;

    public Exception? Exception { get; protected set; }
    public void FaultPipeline(Exception exception) => Exception = exception;

    protected ImmutableArray<(string, IDataflowBlock)> DataflowBlocks { get; }
    protected abstract IEnumerable<(string, IDataflowBlock)> DefinePipelineSections();

    protected void Link<T>(ISourceBlock<T> source, ITargetBlock<T> target, DataflowLinkOptions? options = null)
    {
        options ??= WithCompletionPropagation;
        _links.Add(source.LinkTo(target, options));
    }

    protected void TraceBlockCompletion(string name, IDataflowBlock block)
    {
        _ = block.Completion.ContinueWith(t =>
        {
            var message = t.Status switch
            {
                TaskStatus.RanToCompletion => $"{GetType().Name}[{name}] task completed successfully.",
                TaskStatus.Canceled => $"{GetType().Name}[{name}] task was cancelled.",
                TaskStatus.Faulted => $"{GetType().Name}[{name}] task is faulted.",
                _ => $"{GetType().Name}[{name}] task is in unexpected state '{t.Status}'.",
            };
            LogTrace(message, Exception?.Message);
        }, Token, TaskContinuationOptions.None, TaskScheduler.Default);
    }

    protected void LogPipelineBlockCompletionState()
    {
        var builder = new StringBuilder();
        builder.AppendLine($"Pipeline completion status");

        foreach (var (name, block) in DataflowBlocks)
        {
            builder.AppendLine($"[{name}] status: {block.Completion.Status}");
        }
        LogDebug(builder.ToString());
    }

    protected virtual void FaultDataflowBlock(string? name, IDataflowBlock block, Exception exception)
    {
        block.Fault(exception);

        if (exception is OperationCanceledException)
        {
            LogWarning($"Dataflow block '{name ?? typeof(IDataflowBlock).Name}' was faulted (operation was cancelled).");
        }
        else
        {
            LogException(exception, $"Dataflow block '{name ?? typeof(IDataflowBlock).Name}' was faulted.");
            TokenSource?.Cancel();
        }

        LogPipelineBlockCompletionState();
    }

    protected void ThrowIfCancellationRequested()
    {
        try
        {
            Token.ThrowIfCancellationRequested();
        }
        catch (ObjectDisposedException)
        {
            throw new TaskCanceledException("CancellationTokenSource is already disposed.");
        }
    }

    public void Dispose()
    {
        LogTrace("Disposing pipeline");
        TokenSource?.Dispose();
        foreach (var disposable in _links)
        {
            disposable.Dispose();
        }

        GC.SuppressFinalize(this);
    }
}

public abstract class DataflowPipelineSection<TInput, TState> : DataflowPipeline
{
    private readonly DataflowPipeline _parent;

    protected DataflowPipelineSection(DataflowPipeline parent, 
        ILogger logger, RubberduckSettingsProvider settingsProvider, PerformanceRecordAggregator performance)
        : base(logger, settingsProvider, performance)
    {
        _parent = parent;
    }

    protected GroupingDataflowBlockOptions GreedyJoinExecutionOptions(CancellationToken token) => 
        new() { CancellationToken = token, Greedy = true };
    
    protected ExecutionDataflowBlockOptions ConcurrentExecutionOptions(CancellationToken token) =>
        new() { CancellationToken = token, MaxDegreeOfParallelism = 4 };
    
    protected ExecutionDataflowBlockOptions SingleMessageExecutionOptions(CancellationToken token) =>
        new() { CancellationToken = token, MaxDegreeOfParallelism = 1 };

    protected void FaultParent(Exception exception) => _parent.FaultPipeline(exception);

    protected sealed override void FaultDataflowBlock(string? name, IDataflowBlock block, Exception exception)
    {
        base.FaultDataflowBlock(name, block, exception);
        FaultPipeline(exception);
    }

    protected override IEnumerable<(string, IDataflowBlock)> DefinePipelineSections() => throw new NotSupportedException();
}


public abstract class ParserPipelineSection<TInput, TState> : DataflowPipelineSection<TInput, TState>, IParserPipeline<TInput, TState>, IDisposable
{
    protected ParserPipelineSection(DataflowPipeline parent, 
        ILogger<WorkspaceParserPipeline> logger, RubberduckSettingsProvider settingsProvider, PerformanceRecordAggregator performance)
        : base(parent, logger, settingsProvider, performance)
    {
    }

    public void Cancel() => TokenSource?.Cancel();

    public TState State { get; protected set; } = default!;

    protected virtual void SetInitialState(TState state) => State = state;

    /// <summary>
    /// Defines and links all pipeline blocks for this section.
    /// </summary>
    /// <returns>
    /// Returns the input block at index 0, and the completion task of the section.
    /// </returns>
    protected abstract (IEnumerable<IDataflowBlock>, Task) DefinePipelineBlocks(CancellationTokenSource? tokenSource);

    /// <summary>
    /// Gets the <c>IDataflowBlock</c> that receives the input when the pipeline is started.
    /// </summary>
    /// <remarks>
    /// Useful for linking different pipelines.
    /// </remarks>
    public IDataflowBlock? InputBlock { get; private set; }
    /// <summary>
    /// Gets a <c>Task</c> that completes when all the pipeline dataflow blocks have completed.
    /// </summary>
    public Task Completion { get; private set; } = null!;

    public virtual Task StartAsync(object input, object state, CancellationTokenSource? tokenSource)
    {
        return StartAsync((TInput)input, (TState)state, tokenSource);
    }

    /// <summary>
    /// Starts the pipeline by posting the specified input to the <c>InputBlock</c>.
    /// </summary>
    /// <remarks>
    /// Returns immediately with the <c>Completion</c> task.
    /// </remarks>
    public virtual Task StartAsync(TInput input, TState? state, CancellationTokenSource? tokenSource)
    {
        var (blocks, Completion) = DefinePipelineBlocks(tokenSource);

        if (state != null)
        {
            SetInitialState(state);
        }

        ((ITargetBlock<TInput>)blocks.ElementAt(0)).Post(input);
        InputBlock?.Complete();

        return Completion;
    }

    protected virtual TResult RunTransformBlock<T, TResult>(IDataflowBlock block, T param, Func<T, TResult> action, [CallerMemberName]string? actionName = null) where TResult : class
    {
        TResult? result = null;
        if (State != null && !TryRunAction(() =>
        {
            ThrowIfCancellationRequested();

            result = action.Invoke(param);
            ThrowIfCancellationRequested();

        }, out var exception, actionName) && exception != null)
        {
            FaultDataflowBlock(actionName, block, exception);
        }

        return result ?? throw new InvalidOperationException("Result was unexpectedly null.");
    }

    protected virtual TResult RunTransformBlock<T, TResult>(IDataflowBlock block, T param, Func<T, CancellationToken, TResult> action, [CallerMemberName] string? actionName = null) where TResult : class
    {
        TResult? result = null;
        if (State != null && !TryRunAction(() =>
        {
            ThrowIfCancellationRequested();

            result = action.Invoke(param, Token);
            ThrowIfCancellationRequested();

        }, out var exception, actionName) && exception != null)
        {
            FaultDataflowBlock(actionName, block, exception);
        }

        return result ?? throw new InvalidOperationException("Result was unexpectedly null.");
    }

    protected virtual void RunActionBlock<T>(IDataflowBlock block, T param, Action<T> action, [CallerMemberName] string? actionName = null)
    {
        if (State != null && !TryRunAction(() =>
        {
            ThrowIfCancellationRequested();

            action.Invoke(param);
            ThrowIfCancellationRequested();

        }, out var exception, actionName) && exception != null)
        {
            FaultDataflowBlock(actionName, block, exception);
        }
    }

    protected virtual void RunActionBlock<T>(IDataflowBlock block, T param, Action<T, CancellationToken> action, [CallerMemberName] string? actionName = null)
    {
        if (State != null && !TryRunAction(() =>
        {
            ThrowIfCancellationRequested();
            action.Invoke(param, Token);
            ThrowIfCancellationRequested();

        }, out var exception, actionName) && exception != null)
        {
            FaultDataflowBlock(actionName, block, exception);
        }
    }
}
