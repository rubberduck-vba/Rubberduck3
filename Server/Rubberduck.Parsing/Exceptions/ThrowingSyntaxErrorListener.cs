using Antlr4.Runtime;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.Parsing.Model;

namespace Rubberduck.Parsing.Exceptions;

public class ThrowingSyntaxErrorListener : RubberduckParseErrorListenerBase
{
    public ThrowingSyntaxErrorListener(WorkspaceFileUri uri, CodeKind codeKind) :base(uri, codeKind) { }

    public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
    {
        throw new SyntaxErrorException(
            new AntlrSyntaxErrorInfo
            {
                Uri = Uri,
                CodeKind = CodeKind,

                Message = msg,
                Exception = e,
                OffendingSymbol = offendingSymbol,

                LineNumber = line,
                Position = charPositionInLine,
            });
    }
}
