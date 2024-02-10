using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Symbols;

/// <summary>
/// Represents a symbol that can be resolved to a <c>VBType</c>.
/// </summary>
public interface ITypedSymbol
{
    /// <summary>
    /// Gets the symbol's resolved type.
    /// </summary>
    VBType? ResolvedType { get; }

    /// <summary>
    /// Gets a copy of this symbol with the specified resolved type.
    /// </summary>
    TypedSymbol WithResolvedType(VBType resolvedType);
}
