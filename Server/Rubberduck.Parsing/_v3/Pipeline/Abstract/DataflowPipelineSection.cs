using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks.Dataflow;

namespace Rubberduck.Parsing._v3.Pipeline.Abstract;

public abstract class DataflowPipelineSection<TInput, TState> : DataflowPipeline where TState : class
{
    private readonly DataflowPipeline _parent;

    protected DataflowPipelineSection(DataflowPipeline parent,
        ILogger logger, RubberduckSettingsProvider settingsProvider, PerformanceRecordAggregator performance)
        : base(logger, settingsProvider, performance)
    {
        _parent = parent;
    }

    public TState State { get; protected set; } = default!;

    /// <summary>
    /// Gets the <c>IDataflowBlock</c> that receives the input when the pipeline is started.
    /// </summary>
    /// <remarks>
    /// Useful for linking different pipelines.
    /// </remarks>
    public IDataflowBlock? InputBlock { get; private set; }

    public override Task StartAsync(object input, CancellationTokenSource? tokenSource) => 
        StartAsync((TInput)input, null, tokenSource);

    /// <summary>
    /// Starts the pipeline by posting the specified input to the <c>InputBlock</c>.
    /// </summary>
    /// <remarks>
    /// Returns immediately with the <c>Completion</c> task.
    /// </remarks>
    public virtual async Task StartAsync(TInput input, TState? state, CancellationTokenSource? tokenSource)
    {
        var (blocks, completion) = DefineSectionBlocks(tokenSource);

        Completion = completion;
        if (Completion is null)
        {
            throw new InvalidOperationException($"{nameof(DefineSectionBlocks)} unexpectedly did not return a completion task.");
        }

        if (blocks is null || !blocks.Any())
        {
            throw new InvalidOperationException($"{nameof(DefineSectionBlocks)} unexpectedly did not return any dataflow blocks.");
        }

        if (state != null)
        {
            SetInitialState(state);
        }

        InputBlock = blocks.ElementAt(0);

        ((ITargetBlock<TInput>)InputBlock).Post(input);
        InputBlock.Complete();
        
        await Completion;
    }

    protected virtual void SetInitialState(TState state) => State = state;

    /// <summary>
    /// Defines and links all pipeline blocks for this section.
    /// </summary>
    /// <returns>
    /// Returns the input block at index 0, and the completion task of the section.
    /// </returns>
    protected abstract (IEnumerable<IDataflowBlock> blocks, Task completion) DefineSectionBlocks(CancellationTokenSource? tokenSource);

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

    protected abstract ImmutableArray<(string Name, IDataflowBlock Block)> DataflowBlocks { get; }

    public void LogPipelineCompletionState()
    {
        var builder = new StringBuilder();
        builder.AppendLine($"Pipeline completion status");

        foreach (var (name, block) in DataflowBlocks)
        {
            builder.AppendLine($"[{name}] status: {block.Completion.Status}");
        }
        LogDebug(builder.ToString());
    }

    protected virtual TResult RunTransformBlock<T, TResult>(IDataflowBlock block, T param, Func<T, TResult> action, [CallerMemberName] string? actionName = null, bool logPerformance = true) where TResult : class
    {
        TResult? result = null;
        if (!TryRunAction(() =>
        {
            ThrowIfCancellationRequested();

            result = action.Invoke(param);
            ThrowIfCancellationRequested();

        }, out var exception, actionName, logPerformance) && exception != null)
        {
            FaultDataflowBlock(actionName, block, exception);
        }

        return result ?? throw new InvalidOperationException("Result was unexpectedly null.");
    }

    protected virtual TResult RunTransformBlock<T, TResult>(IDataflowBlock block, T param, Func<T, CancellationToken, TResult> action, [CallerMemberName] string? actionName = null, bool logPerformance = true) where TResult : class
    {
        TResult? result = null;
        if (!TryRunAction(() =>
        {
            ThrowIfCancellationRequested();

            result = action.Invoke(param, Token);
            ThrowIfCancellationRequested();

        }, out var exception, actionName, logPerformance) && exception != null)
        {
            FaultDataflowBlock(actionName, block, exception);
        }

        return result ?? throw new InvalidOperationException("Result was unexpectedly null.");
    }

    protected virtual void RunActionBlock<T>(IDataflowBlock block, T param, Action<T> action, [CallerMemberName] string? actionName = null, bool logPerformance = true)
    {
        if (!TryRunAction(() =>
        {
            ThrowIfCancellationRequested();

            action.Invoke(param);
            ThrowIfCancellationRequested();

        }, out var exception, actionName, logPerformance) && exception != null)
        {
            FaultDataflowBlock(actionName, block, exception);
        }
    }

    protected virtual void RunActionBlock<T>(IDataflowBlock block, T param, Action<T, CancellationToken> action, [CallerMemberName] string? actionName = null, bool logPerformance = true)
    {
        if (!TryRunAction(() =>
        {
            ThrowIfCancellationRequested();
            action.Invoke(param, Token);
            ThrowIfCancellationRequested();

        }, out var exception, actionName, logPerformance) && exception != null)
        {
            FaultDataflowBlock(actionName, block, exception);
        }
    }
}
