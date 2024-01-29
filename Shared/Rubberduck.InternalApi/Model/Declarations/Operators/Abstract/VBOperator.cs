using Rubberduck.InternalApi.Model.Declarations.Execution;
using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;

namespace Rubberduck.InternalApi.Model.Declarations.Operators.Abstract;

public abstract record class VBOperator : TypedSymbol, IExecutableSymbol
{
    protected VBOperator(string token, TypedSymbol[]? operands = null, VBType? type = null)
        : base(RubberduckSymbolKind.Operator, Accessibility.Undefined, token, parentUri: null, operands, type)
    {
    }

    public bool? IsReachable { get; init; }

    public ExecutionContext Execute(ExecutionContext context)
    {
        context.TryRunAction(() => ExecuteOperator(context));
        return context;
    }

    protected abstract ExecutionContext ExecuteOperator(ExecutionContext context);

    public VBOperator WithOperands(TypedSymbol[] operands) => this with { Children = new(operands) };
}
