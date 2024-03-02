using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Rubberduck.InternalApi.Extensions;

namespace Rubberduck.Parsing.Exceptions;

public class ThrowingSyntaxErrorListener : RubberduckParseErrorListenerBase
{
    public ThrowingSyntaxErrorListener(WorkspaceFileUri uri, ISyntaxErrorMessageService? messageService, PredictionMode mode) 
        : base(uri, messageService, mode, throwOnSyntaxError: true) { }
}

