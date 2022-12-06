using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System.Collections.Generic;

namespace Rubberduck.Parsing
{
    public class ParserResult
    {
        public ParserResult(IParseTree parseTree, IEnumerable<SyntaxError> errors, TokenStreamRewriter rewriter)
        {
            ParseTree = parseTree;
            Errors = errors;
            Rewriter = rewriter;
        }

        public IParseTree ParseTree { get; }
        public IEnumerable<SyntaxError> Errors { get; }
        public TokenStreamRewriter Rewriter { get; }
    }
}
