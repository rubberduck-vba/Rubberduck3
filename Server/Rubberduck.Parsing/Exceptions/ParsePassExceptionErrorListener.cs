using Antlr4.Runtime;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.Parsing.Model;

namespace Rubberduck.Parsing.Exceptions;

public class ParsePassExceptionErrorListener : ParsePassErrorListenerBase
{
    public ParsePassExceptionErrorListener(WorkspaceFileUri uri, CodeKind codeKind)
    :base(uri, codeKind) { }

    public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
    {
        // adding 1 to line, because line is 0-based, but it's 1-based in the VBE
        throw new ParsePassSyntaxErrorException(msg, e, offendingSymbol, line, charPositionInLine + 1, Uri, CodeKind);
    }
}
