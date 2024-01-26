namespace Rubberduck.Parsing._v3.Pipeline.Abstract;

public interface IParserPipelineFactory<TPipeline> where TPipeline : IParserPipeline
{
    TPipeline Create();
}
