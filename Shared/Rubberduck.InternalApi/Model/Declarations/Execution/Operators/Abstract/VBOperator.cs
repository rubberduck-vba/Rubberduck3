using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using System;
using System.Linq;

namespace Rubberduck.InternalApi.Model.Declarations.Operators.Abstract;

public abstract record class VBOperator : OperatorSymbol
{
    protected VBOperator(string token, WorkspaceUri parentUri, TypedSymbol[]? operands = null)
        : base(token, parentUri, operands) { }

    public VBOperator WithOperands(TypedSymbol[] operands) => this with { Children = new(operands) };

    public bool CanExecute => Children?.OfType<TypedSymbol>().All(e => e.ResolvedType != null) ?? false;
}
