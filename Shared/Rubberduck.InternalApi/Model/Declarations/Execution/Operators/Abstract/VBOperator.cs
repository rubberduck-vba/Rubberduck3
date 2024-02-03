using Rubberduck.InternalApi.Model.Declarations.Symbols;
using System;

namespace Rubberduck.InternalApi.Model.Declarations.Operators.Abstract;

public abstract record class VBOperator : OperatorSymbol
{
    protected VBOperator(string token, Uri parentUri, TypedSymbol[]? operands = null)
        : base(token, parentUri, operands) { }

    public VBOperator WithOperands(TypedSymbol[] operands) => this with { Children = new(operands) };
}
