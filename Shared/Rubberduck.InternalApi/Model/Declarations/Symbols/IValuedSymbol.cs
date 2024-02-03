using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Symbols;

/// <summary>
/// Represents a typed symbol that defines a value expression that can be resolved to a <c>VBType</c>.
/// </summary>
public interface IValuedSymbol : ITypedSymbol, IValuedExpression
{
    /// <summary>
    /// The declared value expression, if present.
    /// </summary>
    string? ValueExpression { get; }

    /// <summary>
    /// The resolved type of the value expression.
    /// </summary>
    /// <remarks>
    /// May differ from the declared/resolved type of the symbol itself.
    /// </remarks>
    VBType? ResolvedValueExpressionType { get; }
    /// <summary>
    /// Gets a copy of this symbol with the specified resolved value expression type.
    /// </summary>
    ITypedSymbol WithResolvedValueExpressionType(VBType? resolvedValueExpressionType);
}
