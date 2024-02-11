using Rubberduck.InternalApi.Extensions;
using Rubberduck.Parsing.Abstract;
using Rubberduck.Parsing.Model;

namespace Rubberduck.Parsing.Exceptions;

public class ExceptionErrorListenerFactory : IRubberduckParserErrorListenerFactory
{
    public IRubberduckParseErrorListener Create(WorkspaceFileUri uri, CodeKind codeKind)
    {
        return new ExceptionErrorListener(uri, codeKind);
    }
}
