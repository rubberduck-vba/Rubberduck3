using Antlr4.Runtime;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.Parsing.Model;

namespace Rubberduck.Parsing.Exceptions;

public class ReportingSyntaxErrorListener : RubberduckParseErrorListenerBase
{
    public ReportingSyntaxErrorListener(WorkspaceFileUri uri, CodeKind codeKind) :base(uri, codeKind) { }

    public List<AntlrSyntaxErrorInfo> SyntaxErrors { get; } = [];

    public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
    {
        SyntaxErrors.Add(
            new AntlrSyntaxErrorInfo
            {
                Uri = this.Uri,
                CodeKind = this.CodeKind,

                Message = msg, // TODO implement a service dedicated to figuring out syntax error messages; ideally ANTLR error messages never reach the editor.
                Exception = e,
                OffendingSymbol = offendingSymbol,

                LineNumber = line,
                Position = charPositionInLine,
            });
    }
}
