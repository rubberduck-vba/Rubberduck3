using System.Threading;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Rubberduck.Parsing.Abstract;
using Rubberduck.Parsing.Model;

namespace Rubberduck.Parsing.Parsers
{
    public class TokenStreamParserStringParserAdapterWithPreprocessing : IStringParser
    {
        private readonly ICommonTokenStreamProvider<string> _tokenStreamProvider;
        private readonly ITokenStreamParser _tokenStreamParser;
        private readonly ITokenStreamPreprocessor _preprocessor;

        public TokenStreamParserStringParserAdapterWithPreprocessing(ICommonTokenStreamProvider<string> tokenStreamProvider, ITokenStreamParser tokenStreamParser, ITokenStreamPreprocessor preprocessor)
        {
            _tokenStreamProvider = tokenStreamProvider;
            _tokenStreamParser = tokenStreamParser;
            _preprocessor = preprocessor;
        }

        public (IParseTree tree, ITokenStream tokenStream) Parse(string moduleName, string projectId, string code, CancellationToken token,
            CodeKind codeKind = CodeKind.SnippetCode, ParserMode parserMode = ParserMode.FallBackSllToLl)
        {
            token.ThrowIfCancellationRequested();

            var rawTokenStream = _tokenStreamProvider.Tokens(code);
            token.ThrowIfCancellationRequested();

            var tokenStream = _preprocessor.PreprocessTokenStream(projectId, moduleName, rawTokenStream, token, codeKind);
            token.ThrowIfCancellationRequested();

            var tree = _tokenStreamParser.Parse(moduleName, projectId, tokenStream, token, codeKind, parserMode);
            return (tree, tokenStream);
        }
    }
}
