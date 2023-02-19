using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Rubberduck.Parsing.Grammar;

namespace Rubberduck.Parsing.Parsers
{
    public sealed class VBALikePatternParser
    {
        /// <summary>
        /// Parses the given like pattern.
        /// </summary>
        /// <param name="likePattern">The like pattern of the like operation (e.g. in "a Like b" the b)</param>
        /// <returns>The root of the parse tree.</returns>
        public VBALikeParser.LikePatternStringContext Parse(string likePattern)
        {
            var stream = new AntlrInputStream(likePattern);
            var lexer = new VBALikeLexer(stream);
            var tokens = new CommonTokenStream(lexer);
            var parser = new VBALikeParser(tokens);

            VBALikeParser.CompilationUnitContext tree;
            try
            {
                parser.Interpreter.PredictionMode = PredictionMode.Sll;
                tree = parser.compilationUnit();
            }
            catch
            {
                tokens.Reset();
                parser.Reset();
                parser.Interpreter.PredictionMode = PredictionMode.Ll;
                tree = parser.compilationUnit();
            }
            return tree.likePatternString();
        }
    }
}
