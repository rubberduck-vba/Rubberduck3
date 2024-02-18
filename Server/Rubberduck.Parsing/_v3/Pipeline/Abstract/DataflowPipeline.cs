using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using System.Threading.Tasks.Dataflow;

namespace Rubberduck.Parsing._v3.Pipeline.Abstract;

public abstract class DataflowPipeline : ServiceBase, IDisposable
{
    private readonly List<IDisposable> _links = [];
    protected static DataflowLinkOptions WithCompletionPropagation { get; } = new() { PropagateCompletion = true };
    protected static DataflowLinkOptions WithoutCompletionPropagation { get; } = new() { PropagateCompletion = false };


    protected DataflowPipeline(ILogger logger, RubberduckSettingsProvider settingsProvider, PerformanceRecordAggregator performance) 
        : base(logger, settingsProvider, performance)
    {
    }

    public abstract Task StartAsync(object input, object? state, CancellationTokenSource? tokenSource);
    public Task Completion { get; internal set; } = default!;
    public void Cancel() => TokenSource?.Cancel();
    protected virtual CancellationTokenSource? TokenSource { get; set; }
    protected CancellationToken Token => TokenSource?.Token ?? CancellationToken.None;

    public Exception? Exception { get; protected set; }
    public void FaultPipeline(Exception exception) => Exception = exception;

    protected void Link<T>(ISourceBlock<T> source, ITargetBlock<T> target, DataflowLinkOptions? options = null)
    {
        if (source is BroadcastBlock<T>)
        {
            // propagating completion for a broadcast block would complete the first output branch, and leave all others dangling.
            // broadcast block should be explicitly/manually completed upon the completion of all its linked outputs.
            options = WithoutCompletionPropagation;
        }

        options ??= WithCompletionPropagation;
        _links.Add(source.LinkTo(target, options));
    }

    protected Task TraceBlockCompletionAsync<TBlock>(string name, TBlock block) where TBlock : IDataflowBlock
    {
        return block.Completion.ContinueWith(t =>
        {
            var message = t.Status switch
            {
                TaskStatus.RanToCompletion => $"{GetType().Name}[{name}] task completed successfully.",
                TaskStatus.Canceled => $"{GetType().Name}[{name}] task was cancelled.",
                TaskStatus.Faulted => $"{GetType().Name}[{name}] task is faulted.",
                _ => $"{GetType().Name}[{name}] task is in unexpected state '{t.Status}'.",
            };
            LogTrace(message, Exception?.Message);
            LogPipelineCompletionState();
        }, Token, TaskContinuationOptions.None, TaskScheduler.Default);
    }

    protected abstract void LogPipelineCompletionState();

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

        LogPipelineCompletionState();
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
