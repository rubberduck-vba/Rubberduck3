using Rubberduck.Parsing.Abstract;

namespace Rubberduck.Parsing.Expressions;

public sealed class CLngLngLibraryFunctionExpression : Expression
{
    private readonly IExpression _expression;

    public CLngLngLibraryFunctionExpression(IExpression expression)
    {
        _expression = expression;
    }

    public override IValue Evaluate()
    {
        return new CCurLibraryFunctionExpression(_expression).Evaluate();
    }
}
