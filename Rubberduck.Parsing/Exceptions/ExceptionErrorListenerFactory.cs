using Rubberduck.Parsing.Abstract;
using Rubberduck.Parsing.Model;

namespace Rubberduck.Parsing.Exceptions
{
    public class ExceptionErrorListenerFactory : IRubberduckParserErrorListenerFactory
    {
        public IRubberduckParseErrorListener Create(CodeKind codeKind)
        {
            return new ExceptionErrorListener(codeKind);
        }
    }
}
