using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using System.Threading.Tasks.Dataflow;

namespace Rubberduck.Parsing._v3.Pipeline.Abstract;

/// <summary>
/// Represents a series of linked, cancellable, asynchronous operations; starts with a given input and provides a <c>Task</c> to await completion.
/// </summary>
public abstract class DataflowPipeline : ServiceBase, IDisposable
{
    private readonly List<IDisposable> _links = [];
    protected static DataflowLinkOptions WithCompletionPropagation { get; } = new() { PropagateCompletion = true };
    protected static DataflowLinkOptions WithoutCompletionPropagation { get; } = new() { PropagateCompletion = false };


    protected DataflowPipeline(ILogger logger, RubberduckSettingsProvider settingsProvider, PerformanceRecordAggregator performance) 
        : base(logger, settingsProvider, performance)
    {
    }

    public abstract Task StartAsync(object input, CancellationTokenSource? tokenSource);
    public Task Completion { get; internal set; } = default!;
    public void Cancel() => TokenSource?.Cancel();
    protected virtual CancellationTokenSource? TokenSource { get; set; }
    protected CancellationToken Token => TokenSource?.Token ?? CancellationToken.None;

    public Exception? Exception { get; private set; }
    public void FaultPipeline(Exception exception)
    {
        Exception = exception;
        try
        {
            TokenSource?.Cancel();
        }
        catch (ObjectDisposedException) { }
    }

    protected void Link<T>(ISourceBlock<T> source, ITargetBlock<T> target, DataflowLinkOptions? options = null)
    {
        options ??= WithCompletionPropagation;
        _links.Add(source.LinkTo(target, options));
    }

    protected Task TraceBlockCompletionAsync<TBlock>(string name, TBlock block) where TBlock : IDataflowBlock => 
        block.Completion.ContinueWith(t =>
        {
            var message = t.Status switch
            {
                TaskStatus.RanToCompletion => $"{GetType().Name}[{name} block] task completed successfully.",
                TaskStatus.Canceled => $"{GetType().Name}[{name} block] task was cancelled.",
                TaskStatus.Faulted => $"{GetType().Name}[{name} block] task is faulted.",
                _ => $"{GetType().Name}[{name} block] task is in unexpected state '{t.Status}'.",
            };
            LogTrace(message, Exception?.Message);
        }, Token, TaskContinuationOptions.None, TaskScheduler.Default);

    protected virtual void FaultDataflowBlock(string? name, IDataflowBlock block, Exception exception)
    {
        block.Fault(exception);

        if (exception is OperationCanceledException || exception is TaskCanceledException)
        {
            LogWarning($"Dataflow block '{name ?? typeof(IDataflowBlock).Name}' was faulted (task or operation was cancelled).");
        }
        else
        {
            LogException(exception, $"Dataflow block '{name ?? typeof(IDataflowBlock).Name}' was faulted.");
            FaultPipeline(exception);
        }
    }

    protected void ThrowIfCancellationRequested()
    {
        try
        {
            Token.ThrowIfCancellationRequested();
        }
        catch (ObjectDisposedException) { }
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
