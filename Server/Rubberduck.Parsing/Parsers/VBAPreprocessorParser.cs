using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Rubberduck.Parsing.Abstract;
using Rubberduck.Parsing.Grammar;

namespace Rubberduck.Parsing.Parsers;

public class VBAPreprocessorParser : TokenStreamParserBase<VBAConditionalCompilationParser>
{
    public VBAPreprocessorParser(IParsePassErrorListenerFactory sllErrorListenerFactory, IParsePassErrorListenerFactory llErrorListenerFactory)
        : base(sllErrorListenerFactory, llErrorListenerFactory)
    {
    }

    protected override VBAConditionalCompilationParser GetParser(ITokenStream tokenStream) => new VBAConditionalCompilationParser(tokenStream);

    protected override IParseTree Parse(VBAConditionalCompilationParser parser) => parser.compilationUnit();
}
