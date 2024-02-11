using Antlr4.Runtime;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.Parsing.Model;

namespace Rubberduck.Parsing.Exceptions;

/// <summary>
/// An exception that is thrown when the parser encounters a syntax error during one of two parses of an entire module.
/// This exception indicates either a bug in the grammar... or non-compilable VBA code.
/// </summary>
[Serializable]
public class ParsePassSyntaxErrorException : SyntaxErrorException
{
    public ParsePassSyntaxErrorException(ParsePassSyntaxErrorInfo info)
        : this(info.Message, info.Exception, info.OffendingSymbol, info.LineNumber, info.Position, info.Uri, info.CodeKind) { }

    public ParsePassSyntaxErrorException(string message, RecognitionException innerException, IToken offendingSymbol, int line, int position, WorkspaceFileUri uri, CodeKind codeKind)
        : base(message, innerException, offendingSymbol, line, position, codeKind)
    {
        Uri = uri;
    }

    public WorkspaceFileUri Uri { get; }

    public override string ToString()
    {
        var parsePassText = CodeKind == CodeKind.CodePaneCode ? "code pane" : "exported";
        var exceptionText = 
$@"{base.ToString()}
Uri: {Uri} ({parsePassText} version)";
        return exceptionText;
    }
}
