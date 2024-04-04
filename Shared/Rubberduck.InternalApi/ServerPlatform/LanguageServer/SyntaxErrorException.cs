using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model;
using System;

namespace Rubberduck.InternalApi.ServerPlatform.LanguageServer;

/// <summary>
/// An exception that is thrown when the parser encounters a syntax error.
/// This exception indicates either a bug in the grammar... or non-compilable VBA code.
/// </summary>
[Serializable]
public class SyntaxErrorException : PredictionFailException
{
    public SyntaxErrorException(WorkspaceFileUri uri, SyntaxErrorOffendingSymbol symbol, string message, Exception? inner = null)
        : base(uri, "ll", symbol, message, inner)
    {
    }

    public override Diagnostic ToDiagnostic() => RubberduckDiagnostic.SyntaxError(this);
}

[Serializable]
public abstract class PredictionFailException : Exception
{
    protected PredictionFailException(WorkspaceFileUri uri, string mode, SyntaxErrorOffendingSymbol symbol, string message, Exception? inner)
        : base(message, inner)
    {
        Uri = uri;
        PredictionMode = mode;
        OffendingSymbol = symbol;
    }

    public SyntaxErrorOffendingSymbol OffendingSymbol { get; init; }
    public string PredictionMode { get; init; }
    public WorkspaceFileUri Uri { get; init; }

    public virtual Diagnostic ToDiagnostic() => RubberduckDiagnostic.SllFailure(this);

    public override string ToString() =>
        $"{base.ToString()}\nUri: {Uri} Token: {OffendingSymbol.Name} at L{OffendingSymbol.Range.Start.Line}C{OffendingSymbol.Range.Start.Character}";
}