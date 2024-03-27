using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;

namespace Rubberduck.Parsing.Exceptions;

/// <summary>
/// An exception that is thrown when the parser fails in SLL prediction mode, which ultimately gets reported as a diagnostic.
/// </summary>
public class SllPredictionFailException : SyntaxErrorException
{
    public SllPredictionFailException(WorkspaceFileUri uri, SyntaxErrorOffendingSymbol symbol, string message, Exception? inner = null) 
        : base(uri, symbol, message, inner) { }

    public override Diagnostic ToDiagnostic() => RubberduckDiagnostic.SllFailure(this);
}
