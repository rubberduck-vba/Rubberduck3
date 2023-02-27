using Rubberduck.Parsing.Abstract;
using Rubberduck.Parsing.Model;

namespace Rubberduck.Parsing.Exceptions
{
    public class ExceptionErrorListenerFactory : IRubberduckParserErrorListenerFactory
    {
        public IRubberduckParseErrorListener Create(string moduleName, CodeKind codeKind)
        {
            return new ExceptionErrorListener(moduleName, codeKind);
        }
    }
}
