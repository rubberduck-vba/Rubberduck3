using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Rubberduck.Parsing.Abstract;
using Rubberduck.Parsing.Grammar;

namespace Rubberduck.Parsing.Parsers
{
    public class VBATokenStreamParser : TokenStreamParserBase<VBAParser>
    {
        public VBATokenStreamParser(IParsePassErrorListenerFactory sllErrorListenerFactory, IParsePassErrorListenerFactory llErrorListenerFactory) 
        :base(sllErrorListenerFactory, llErrorListenerFactory) { }

        protected override VBAParser GetParser(ITokenStream tokenStream) => new VBAParser(tokenStream);

        protected override IParseTree Parse(VBAParser parser) => parser.startRule();
    }

    public class VBAMemberTokenStreamParser : TokenStreamParserBase<VBAMemberParser>
    {
        public VBAMemberTokenStreamParser(IParsePassErrorListenerFactory sllErrorListenerFactory, IParsePassErrorListenerFactory llErrorListenerFactory) 
            : base(sllErrorListenerFactory, llErrorListenerFactory) { }

        protected override VBAMemberParser GetParser(ITokenStream tokenStream) => new VBAMemberParser(tokenStream);

        protected override IParseTree Parse(VBAMemberParser parser) => parser.startRule();
    }
}
