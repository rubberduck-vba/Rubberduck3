using Antlr4.Runtime;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.Parsing.Model;

namespace Rubberduck.Parsing.Exceptions;

/// <summary>
/// An exception that is thrown when the parser encounters a syntax error.
/// This exception indicates either a bug in the grammar... or non-compilable VBA code.
/// </summary>
[Serializable]
public class SyntaxErrorException : Exception
{
    public SyntaxErrorException(AntlrSyntaxErrorInfo info)
        : this(info.Uri, info.Message, info.Exception, info.OffendingSymbol, info.LineNumber, info.Position) { }

    public SyntaxErrorException(WorkspaceFileUri uri, string message, RecognitionException innerException, IToken offendingSymbol, int line, int position)
        : base(message, innerException)
    {
        Uri = uri;

        OffendingSymbol = offendingSymbol;
        LineNumber = line;
        Position = position;
    }

    public WorkspaceFileUri Uri { get; init; }

    public IToken OffendingSymbol { get; init; }
    public int LineNumber { get; init; }
    public int Position { get; init; }

    public override string ToString() => $"{base.ToString()}\nToken: {OffendingSymbol.Text} at L{LineNumber}C{Position}";
}
