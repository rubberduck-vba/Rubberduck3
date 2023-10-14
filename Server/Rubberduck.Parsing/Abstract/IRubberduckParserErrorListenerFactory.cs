using Rubberduck.Parsing.Model;

namespace Rubberduck.Parsing.Abstract;

public interface IRubberduckParserErrorListenerFactory
{
    IRubberduckParseErrorListener Create(string moduleName, CodeKind codeKind);
}
