namespace Rubberduck.InternalApi.Model.Declarations;

/// <summary>
/// Represents a symbol that can be annotated.
/// </summary>
public interface IAnnotatedSymbol
{
    /// <summary>
    /// The annotations associated with the symbol, if any.
    /// </summary>
    IParseTreeAnnotation[] Annotations { get; }
}