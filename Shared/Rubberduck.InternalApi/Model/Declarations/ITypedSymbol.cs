using Rubberduck.InternalApi.Model.Declarations.Types;

namespace Rubberduck.InternalApi.Model.Declarations;

public interface ITypedSymbol<T> where T : Symbol
{
    /// <summary>
    /// Gets the symbol's declared type expression, as it appears in the source.
    /// </summary>
    string AsTypeExpression { get; }
    /// <summary>
    /// Gets the symbol's resolved type.
    /// </summary>
    VBType? ResolvedType { get; }

    /// <summary>
    /// Gets a copy of this symbol with the specified resolved type.
    /// </summary>
    T ResolveType(VBType? resolvedType);
}
