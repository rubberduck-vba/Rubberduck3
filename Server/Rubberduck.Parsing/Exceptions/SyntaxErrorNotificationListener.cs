using Antlr4.Runtime;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.Parsing.Model;
using System.Collections.Immutable;

namespace Rubberduck.Parsing.Exceptions;

public class SyntaxErrorNotificationListener : RubberduckParseErrorListenerBase
{
    private readonly IList<AntlrSyntaxErrorInfo> _errors = new List<AntlrSyntaxErrorInfo>();

    public SyntaxErrorNotificationListener(WorkspaceFileUri uri, ISyntaxErrorMessageService messageService) 
    :base(uri, messageService) { }

    protected override List<AntlrSyntaxErrorInfo> Errors { get; } = [];

    public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
    {
        var info = GetErrorInfo(offendingSymbol, line, charPositionInLine, msg, e);
        _errors.Add(info);
    }
}