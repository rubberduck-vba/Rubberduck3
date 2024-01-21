using Rubberduck.InternalApi.Model.Declarations.Types;

namespace Rubberduck.InternalApi.Model.Declarations;

public interface IValuedSymbol<T> : ITypedSymbol<T> where T : Symbol
{
    string? ValueExpression { get; }

    VBType? ResolvedValueExpressionType { get; }
    T ResolveValueExpressionType(VBType? resolvedValueExpressionType);

    object? ResolvedValue { get; }
    T ResolveValue(object? resolvedValue);
}
