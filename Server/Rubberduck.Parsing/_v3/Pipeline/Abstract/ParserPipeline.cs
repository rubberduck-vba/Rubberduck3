using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using System.Threading.Tasks.Dataflow;

namespace Rubberduck.Parsing._v3.Pipeline.Abstract;

public interface IParserPipeline
{
    /// <summary>
    /// Starts executing the parser pipeline for the provided input.
    /// </summary>
    Task StartAsync(object input);
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
    Task StartAsync(TInput input);
}

public abstract class ParserPipeline<TInput, TState> : ServiceBase, IParserPipeline<TInput>, IDisposable
{
    protected static DataflowLinkOptions WithCompletionPropagation { get; } = new() { PropagateCompletion = true };
    protected static DataflowLinkOptions WithoutCompletionPropagation { get; } = new() { PropagateCompletion = false };

    protected ExecutionDataflowBlockOptions ExecutionOptions { get; }

    private readonly CancellationTokenSource _tokenSource;
    private readonly List<IDisposable> _disposables = [];

    protected ParserPipeline(ILogger<WorkspaceParserPipeline> logger,
        RubberduckSettingsProvider settingsProvider,
        PerformanceRecordAggregator performance)
        : base(logger, settingsProvider, performance)
    {
        _tokenSource = new();

        ExecutionOptions = new() 
        { 
            CancellationToken = _tokenSource.Token,
            MaxDegreeOfParallelism = 4, // TODO make this configurable
        };

        (InputBlock, Completion) = DefinePipelineBlocks();
    }

    protected CancellationToken Token => _tokenSource.Token;
    protected TState? State { get; set; }

    protected abstract void SetInitialState(TInput input);

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

    /// <summary>
    /// Gets the <c>IDataflowBlock</c> that produces the output for this part of the processing.
    /// </summary>
    /// <remarks>
    /// Useful for linking different pipelines.
    /// </remarks>
    public IDataflowBlock OutputBlock { get; }

    public Task Completion { get; }

    public virtual Task StartAsync(object input) => StartAsync((TInput)input);

    /// <summary>
    /// Starts the pipeline by posting the specified input to the <c>InputBlock</c>.
    /// </summary>
    /// <remarks>
    /// Returns immediately with the <c>Completion</c> task.
    /// </remarks>
    public virtual Task StartAsync(TInput input)
    {
        if (State != null)
        {
            throw new InvalidOperationException("BUG: This pipeline instance is already in use; a new one should be created every time.");
        }

        SetInitialState(input);

        ((ITargetBlock<TInput>)InputBlock).Post(input);
        InputBlock.Complete();

        return Completion;
    }

    protected void Link<T>(ISourceBlock<T> source, ITargetBlock<T> target, DataflowLinkOptions options)
    {
        _disposables.Add(source.LinkTo(target, options));
    }

    protected void FaultDataflowBlock(IDataflowBlock block, Exception exception)
    {
        block.Fault(exception);
        _tokenSource?.Cancel();
    }

    protected void ThrowIfCancellationRequested() => _tokenSource.Token.ThrowIfCancellationRequested();

    public void Dispose()
    {
        _tokenSource?.Dispose();
        foreach (var disposable in _disposables)
        {
            disposable.Dispose();
        }
    }
}
