using Rubberduck.Parsing.Model;

namespace Rubberduck.Parsing.Abstract
{
    public interface IParsePassErrorListenerFactory
    {
        IRubberduckParseErrorListener Create(string moduleName, CodeKind codeKind);
    }
}
