using Rubberduck.InternalApi.Extensions;
using Rubberduck.Parsing._v3.Pipeline.Abstract;
using System.Collections.Concurrent;

namespace Rubberduck.Parsing._v3.Pipeline;

public class ParserPipelineProvider : IParserPipelineProvider<WorkspaceUri>, IParserPipelineProvider<WorkspaceFileUri>
{
    private readonly IParserPipelineFactory<WorkspaceParserPipeline> _workspacePipelineFactory;
    private readonly IParserPipelineFactory<WorkspaceFileParserPipeline> _filePipelineFactory;
    private readonly ConcurrentDictionary<Uri, IParserPipeline> _pipelines = [];
    private readonly ConcurrentDictionary<Uri, Task> _tasks = [];

    public ParserPipelineProvider(IParserPipelineFactory<WorkspaceParserPipeline> workspacePipelineFactory,
        IParserPipelineFactory<WorkspaceFileParserPipeline> filePipelineFactory)
    {
        _workspacePipelineFactory = workspacePipelineFactory;
        _filePipelineFactory = filePipelineFactory;
    }

    private IParserPipeline<TInput> StartNew<TPipeline, TInput>(IParserPipelineFactory<TPipeline> factory, TInput uri, CancellationTokenSource? tokenSource = null) 
        where TPipeline : IParserPipeline
        where TInput : Uri
    {
        _ = uri ?? throw new ArgumentNullException(nameof(uri));

        if (_pipelines.TryGetValue(uri, out var pipeline))
        {
            try
            {
                pipeline.Cancel();
            }
            finally
            {
                _tasks.Remove(uri, out _);
            }
        }

        var newPipeline = factory.Create() as IParserPipeline<TInput> ?? throw new InvalidOperationException("BUG: IParserPipeline was not of the expected type.");
        var completion = newPipeline.StartAsync(uri, tokenSource) ?? throw new InvalidOperationException("BUG: Completion Task was unexpectly null.");
        
        _tasks.TryAdd(uri, completion);
        _pipelines[uri] = newPipeline;

        return newPipeline;
    }

    public IParserPipeline<WorkspaceUri> StartNew(WorkspaceUri uri, CancellationTokenSource? tokenSource = null) =>
        StartNew(_workspacePipelineFactory, uri, tokenSource);

    public IParserPipeline<WorkspaceFileUri> StartNew(WorkspaceFileUri uri, CancellationTokenSource? tokenSource = null) =>
        StartNew(_filePipelineFactory, uri, tokenSource);

    public IParserPipeline<WorkspaceUri>? GetCurrent(WorkspaceUri uri) =>
        _pipelines.TryGetValue(uri, out var current) && current is IParserPipeline<WorkspaceUri> result ? result : null;

    public IParserPipeline<WorkspaceFileUri>? GetCurrent(WorkspaceFileUri uri) =>
        _pipelines.TryGetValue(uri, out var current) && current is IParserPipeline<WorkspaceFileUri> result ? result : null;
}
