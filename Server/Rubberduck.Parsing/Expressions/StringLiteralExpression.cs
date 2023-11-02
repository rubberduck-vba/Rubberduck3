using Rubberduck.Parsing.Abstract;

namespace Rubberduck.Parsing.Expressions;

public sealed class StringLiteralExpression : Expression
{
    private readonly IExpression _tokenText;

    public StringLiteralExpression(IExpression tokenText)
    {
        _tokenText = tokenText;
    }

    public override IValue Evaluate()
    {
        var str = _tokenText.Evaluate().AsString;
        // Remove quotes
        str = str[1..^1];
        return new StringValue(str);
    }
}
