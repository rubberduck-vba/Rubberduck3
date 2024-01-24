using Antlr4.Runtime;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.Parsing.Model;

namespace Rubberduck.Parsing.Exceptions;

/// <summary>
/// An exception that is thrown when the parser encounters a syntax error while parsing an entire module.
/// This exception indicates either a bug in the grammar... or non-compilable VBA code.
/// </summary>
[Serializable]
public class MainParseSyntaxErrorException : ParsePassSyntaxErrorException
{
    public MainParseSyntaxErrorException(ParsePassSyntaxErrorInfo info)
        : this(info.Message, info.Exception, info.OffendingSymbol, info.LineNumber, info.Position, info.Uri, info.CodeKind) { }

    public MainParseSyntaxErrorException(string message, RecognitionException innerException, IToken offendingSymbol, int line, int position, WorkspaceFileUri uri, CodeKind codeKind)
        : base(message, innerException, offendingSymbol, line, position, uri, codeKind)
    {}

    public override string ToString()
    {
        var exceptionText = 
$@"{base.ToString()}
ParseType: Main parse";
        return exceptionText;
    }
}
