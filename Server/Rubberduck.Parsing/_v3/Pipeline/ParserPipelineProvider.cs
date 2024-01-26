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

    public IParserPipeline<TInput> GetCurrentOrStartNew<TInput>(WorkspaceUri uri, TInput input)
    {
        _ = input ?? throw new ArgumentNullException(nameof(input));

        if (!_pipelines.TryGetValue(uri, out var pipeline))
        {
            pipeline = _workspacePipelineFactory.Create();
        }

        _pipelines[uri] = pipeline ?? throw new InvalidOperationException();
        
        if (pipeline is IParserPipeline<TInput> parserPipeline)
        {
            _ = pipeline.StartAsync(input);
            return parserPipeline;
        }

        throw new InvalidCastException($"Expected `IParser<{typeof(TInput).Name}>` but was `{pipeline?.GetType().Name ?? "(null)"}`.");
    }

    public IParserPipeline<TInput>? GetCurrent<TInput>(WorkspaceUri uri)
    {
        if (_pipelines.TryGetValue(uri, out var current) && current is IParserPipeline<TInput> result)
        {
            return result;
        }

        return null;
    }

    public IParserPipeline<TInput> GetCurrentOrStartNew<TInput>(WorkspaceFileUri uri, TInput input)
    {
        _ = input ?? throw new ArgumentNullException(nameof(input));

        if (!_pipelines.TryGetValue(uri, out var pipeline))
        {
            pipeline = _filePipelineFactory.Create();
        }

        _pipelines[uri] = pipeline;
        _ = pipeline.StartAsync(uri);

        return (IParserPipeline<TInput>)pipeline;
    }

    public IParserPipeline<TInput>? GetCurrent<TInput>(WorkspaceFileUri uri)
    {
        if (_pipelines.TryGetValue(uri, out var current) && current is IParserPipeline<TInput> result)
        {
            return result;
        }

        return null;
    }
}
