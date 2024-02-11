using Antlr4.Runtime;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.Parsing.Model;

namespace Rubberduck.Parsing.Exceptions;

public class SyntaxErrorNotificationListener : RubberduckParseErrorListenerBase
{
    private readonly IList<SyntaxErrorInfo> _errors = new List<SyntaxErrorInfo>();

    public SyntaxErrorNotificationListener(WorkspaceFileUri uri, CodeKind codeKind) 
    :base(uri, codeKind) { }

    public IEnumerable<SyntaxErrorInfo> Errors => _errors;

    public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
    {
        _errors.Add(new SyntaxErrorInfo(msg, e, offendingSymbol, line, charPositionInLine, Uri, CodeKind));
    }
}