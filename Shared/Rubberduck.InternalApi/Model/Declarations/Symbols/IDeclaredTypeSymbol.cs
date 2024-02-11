namespace Rubberduck.InternalApi.Model.Declarations.Symbols;

/// <summary>
/// Represents a typed symbol that can include an <c>AsTypeExpression</c>.
/// </summary>
public interface IDeclaredTypeSymbol : ITypedSymbol
{
    /// <summary>
    /// Gets the symbol's declared type expression, as it appears in the source.
    /// </summary>
    /// <remarks>
    /// <c>null</c> if the expression is implicitly defined; the resolved type should then be <c>VBVariant(VBEmpty)</c>.
    /// </remarks>
    string? AsTypeExpression { get; }
}
