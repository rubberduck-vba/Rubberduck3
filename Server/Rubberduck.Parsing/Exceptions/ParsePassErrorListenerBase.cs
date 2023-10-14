using Rubberduck.Parsing.Model;

namespace Rubberduck.Parsing.Exceptions;

public class ParsePassErrorListenerBase : RubberduckParseErrorListenerBase
{
    public ParsePassErrorListenerBase(string moduleName, CodeKind codeKind) 
    :base(moduleName, codeKind)
    {
    }
}
