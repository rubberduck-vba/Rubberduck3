using Antlr4.Runtime;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.Parsing.Model;

namespace Rubberduck.Parsing.Exceptions;

public class SyntaxErrorNotificationListener : RubberduckParseErrorListenerBase
{
    private readonly IList<AntlrSyntaxErrorInfo> _errors = new List<AntlrSyntaxErrorInfo>();

    public SyntaxErrorNotificationListener(WorkspaceFileUri uri, CodeKind codeKind) 
    :base(uri, codeKind) { }

    public IEnumerable<AntlrSyntaxErrorInfo> Errors => _errors;

    public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
    {
        _errors.Add(new AntlrSyntaxErrorInfo
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