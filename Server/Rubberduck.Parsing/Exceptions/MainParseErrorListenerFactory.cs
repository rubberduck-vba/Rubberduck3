using Rubberduck.Parsing.Abstract;
using Rubberduck.Parsing.Model;

namespace Rubberduck.Parsing.Exceptions;

public class MainParseErrorListenerFactory : IParsePassErrorListenerFactory
{
    public IRubberduckParseErrorListener Create(string moduleName, CodeKind codeKind)
    {
        return new MainParseExceptionErrorListener(moduleName, codeKind);
    }
}
