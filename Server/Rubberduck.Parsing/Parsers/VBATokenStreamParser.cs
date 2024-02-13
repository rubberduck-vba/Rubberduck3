using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Rubberduck.Parsing.Abstract;
using Rubberduck.Parsing.Grammar;

namespace Rubberduck.Parsing.Parsers;

public class VBATokenStreamParser : TokenStreamParserBase<VBAParser>
{
    protected override VBAParser GetParser(ITokenStream tokenStream) => new(tokenStream);

    protected override IParseTree Parse(VBAParser parser) => parser.startRule();
}

public class VBAMemberTokenStreamParser : TokenStreamParserBase<VBAMemberParser>
{
    protected override VBAMemberParser GetParser(ITokenStream tokenStream) => new(tokenStream);

    protected override IParseTree Parse(VBAMemberParser parser) => parser.startRule();
}
