using Antlr4.Runtime;
using Rubberduck.Parsing.Abstract;
using Rubberduck.Parsing.Grammar;

namespace Rubberduck.Parsing.TokenStreamProviders;

public abstract class CommonTokenStreamProvider<TContent> : ICommonTokenStreamProvider<TContent>
{
    protected abstract AntlrInputStream GetInputStream(TContent content);

    public CommonTokenStream Tokens(TContent content)
    {
        var stream = GetInputStream(content);
        return Tokenize(stream);
    }

    private CommonTokenStream Tokenize(AntlrInputStream stream)
    {
        var lexer = new VBALexer(stream);
        return new CommonTokenStream(lexer);
    }
}
