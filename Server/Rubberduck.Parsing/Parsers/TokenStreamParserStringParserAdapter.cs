using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Rubberduck.Parsing.Abstract;
using Rubberduck.Parsing.Model;

namespace Rubberduck.Parsing.Parsers;

public class TokenStreamParserStringParserAdapter : IStringParser
{
    private readonly ICommonTokenStreamProvider<string> _tokenStreamProvider;
    private readonly ITokenStreamParser _tokenStreamParser;

    public TokenStreamParserStringParserAdapter(ICommonTokenStreamProvider<string> tokenStreamProvider, ITokenStreamParser tokenStreamParser)
    {
        _tokenStreamProvider = tokenStreamProvider;
        _tokenStreamParser = tokenStreamParser;
    }

    public (IParseTree tree, ITokenStream tokenStream, LogicalLineStore? logicalLines) Parse(string moduleName, string projectId, string content, CancellationToken token, CodeKind codeKind, ParserMode parserMode, IEnumerable<IParseTreeListener>? parseListeners = null)
    {
        token.ThrowIfCancellationRequested();
        var tokenStream = _tokenStreamProvider.Tokens(content);

        token.ThrowIfCancellationRequested();

        var tree = _tokenStreamParser.Parse(moduleName, projectId, tokenStream, token, codeKind, parserMode, parseListeners);
        return (tree, tokenStream, null);
    }
}
