using Microsoft.Extensions.DependencyInjection;
using Rubberduck.Parsing._v3.Pipeline.Abstract;

namespace Rubberduck.Parsing._v3.Pipeline;

public class ParserPipelineFactory<TPipeline> : IParserPipelineFactory<TPipeline> 
    where TPipeline : IParserPipeline
{
    private readonly IServiceProvider _provider;
    public ParserPipelineFactory(IServiceProvider provider)
    {
        _provider = provider;
    }

    public TPipeline Create()
    {
        return _provider.GetRequiredService<TPipeline>();
    }
}
