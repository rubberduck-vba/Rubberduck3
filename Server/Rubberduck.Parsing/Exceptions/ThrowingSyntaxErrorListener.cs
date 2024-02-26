using Antlr4.Runtime;
using Rubberduck.InternalApi.Extensions;

namespace Rubberduck.Parsing.Exceptions;

public class ThrowingSyntaxErrorListener : RubberduckParseErrorListenerBase
{
    public ThrowingSyntaxErrorListener(WorkspaceFileUri uri, ISyntaxErrorMessageService? messageService) 
        :base(uri, messageService) { }

    protected override List<AntlrSyntaxErrorInfo> Errors { get; } = [];

    public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
    {
        var info = GetErrorInfo(offendingSymbol, line, charPositionInLine, msg, e);
        throw new SyntaxErrorException(info);
    }
}
