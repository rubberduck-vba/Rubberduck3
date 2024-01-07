using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Rubberduck.Parsing.Abstract;
using Rubberduck.Parsing.Model;

namespace Rubberduck.Parsing.Parsers;

public class TokenStreamParserAdapterWithPreprocessing<TContent> : IParser<TContent>
{
    private readonly ICommonTokenStreamProvider<TContent> _tokenStreamProvider;
    private readonly ITokenStreamParser _tokenStreamParser;
    private readonly ITokenStreamPreprocessor _preprocessor;

    public TokenStreamParserAdapterWithPreprocessing(ICommonTokenStreamProvider<TContent> tokenStreamProvider, ITokenStreamParser tokenStreamParser, ITokenStreamPreprocessor preprocessor)
    {
        _tokenStreamProvider = tokenStreamProvider;
        _tokenStreamParser = tokenStreamParser;
        _preprocessor = preprocessor;
    }

    public (IParseTree tree, ITokenStream tokenStream, LogicalLineStore? logicalLines) Parse(
        string moduleName, 
        string projectId, 
        TContent content, 
        CancellationToken token,
        CodeKind codeKind = CodeKind.SnippetCode, 
        ParserMode parserMode = ParserMode.FallBackSllToLl,
        IEnumerable<IParseTreeListener>? parseListeners = null)
    {
        token.ThrowIfCancellationRequested();

        var rawTokenStream = _tokenStreamProvider.Tokens(content);
        token.ThrowIfCancellationRequested();

        var tokenStream = _preprocessor.PreprocessTokenStream(projectId, moduleName, rawTokenStream, token, codeKind);
        token.ThrowIfCancellationRequested();

        var tree = _tokenStreamParser.Parse(moduleName, projectId, tokenStream ?? rawTokenStream, token, codeKind, parserMode, parseListeners);
        return (tree, tokenStream ?? rawTokenStream, null);
    }
}
