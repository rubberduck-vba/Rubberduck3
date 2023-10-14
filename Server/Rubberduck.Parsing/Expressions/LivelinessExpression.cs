using Antlr4.Runtime;
using Rubberduck.Parsing.Abstract;

namespace Rubberduck.Parsing.Expressions;

public sealed class LivelinessExpression : Expression
{
    private readonly IExpression _isAlive;
    private readonly IExpression _tokens;

    public LivelinessExpression(IExpression isAlive, IExpression tokens)
    {
        _isAlive = isAlive;
        _tokens = tokens;
    }

    public override IValue Evaluate()
    {
        var isAlive = _isAlive.Evaluate().AsBool;
        var tokens = _tokens.Evaluate().AsTokens;
        if (!isAlive)
        {
            HideDeadTokens(tokens);
        }
        return new TokensValue(tokens);
    }

    private void HideDeadTokens(IEnumerable<IToken> deadTokens)
    {
        foreach(var token in deadTokens)
        {
            //We need this cast because the IToken interface does not expose the setters for the properties.
            //CommonToken is the default token type used by Antlr. (Any custom token types should extend it.)
            if (token is CommonToken commonToken)
            {
                HideNonNewline(commonToken);
            }
        }
    }

    private void HideNonNewline(CommonToken token)
    {
        //We do not remove the newlines or line continuations to keep physical and logical line counts intact.
        if (token.Type != Grammar.VBALexer.NEWLINE && token.Type != Grammar.VBALexer.LINE_CONTINUATION)
        {
            token.Channel = TokenConstants.HiddenChannel;
        }
    }
}
