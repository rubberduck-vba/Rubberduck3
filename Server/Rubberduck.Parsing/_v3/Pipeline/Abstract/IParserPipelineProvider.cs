namespace Rubberduck.Parsing._v3.Pipeline.Abstract;

public interface IParserPipelineProvider<TInput> where TInput : Uri
{
    /// <summary>
    /// Cancels the current parser pipeline for the specified workspace URI if it exists; creates, starts, and returns a new pipeline.
    /// </summary>
    IParserPipeline<TInput> StartNew(TInput uri, CancellationTokenSource? tokenSource = null);
    /// <summary>
    /// Gets the current parser pipeline for the specified worksapcae URI.
    /// </summary>
    /// <returns><c>null</c> if the provided workspace URI was never processed.</returns>
    IParserPipeline<TInput>? GetCurrent(TInput uri);
}
