using Antlr4.Runtime;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.Parsing.Model;

namespace Rubberduck.Parsing.Exceptions;

public class ParsePassSyntaxErrorInfo : SyntaxErrorInfo
{
    public ParsePassSyntaxErrorInfo(string message, RecognitionException innerException, IToken offendingSymbol, int line, int position, WorkspaceFileUri uri, CodeKind codeKind)
        :base(message, innerException, offendingSymbol, line, position, uri, codeKind) { }
}