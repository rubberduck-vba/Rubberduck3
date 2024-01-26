namespace Rubberduck.Parsing._v3.Pipeline.Abstract;

public interface IParserPipelineProvider<TKey> where TKey : Uri
{
    /// <summary>
    /// Gets the current parser pipeline for the specified workspace URI; creates and starts a new pipeline if none exist.
    /// </summary>
    IParserPipeline<TInput> GetCurrentOrStartNew<TInput>(TKey uri, TInput input);
    /// <summary>
    /// Gets the current parser pipeline for the specified worksapcae URI.
    /// </summary>
    /// <returns><c>null</c> if the provided workspace URI was never processed.</returns>
    IParserPipeline<TInput>? GetCurrent<TInput>(TKey uri);
}
