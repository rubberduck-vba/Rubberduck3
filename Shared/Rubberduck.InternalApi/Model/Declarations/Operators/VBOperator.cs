using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;

namespace Rubberduck.InternalApi.Model.Declarations.Operators;

public abstract record class VBOperator : TypedSymbol
{
    protected VBOperator(string token, TypedSymbol[]? operands = null, VBType? type = null) 
        : base(RubberduckSymbolKind.Operator, Accessibility.Undefined, token, parentUri: null, operands, type)
    {
    }

    public VBOperator WithOperands(TypedSymbol[] operands) => this with { Children = new(operands) };
}
