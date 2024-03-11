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
public class SyntaxErrorException : Exception
{
    public SyntaxErrorException(WorkspaceFileUri uri, SyntaxErrorOffendingSymbol symbol, string message, Exception? inner = null)
        : base(message, inner)
    {
        Uri = uri;
        OffendingSymbol = symbol;
    }

    public WorkspaceFileUri Uri { get; init; }

    public SyntaxErrorOffendingSymbol OffendingSymbol { get; init; }

    public virtual Diagnostic ToDiagnostic() => RubberduckDiagnostic.SyntaxError(this);

    public override string ToString() => 
        $"{base.ToString()}\nUri: {Uri} Token: {OffendingSymbol.Name} at L{OffendingSymbol.Range.Start.Line}C{OffendingSymbol.Range.Start.Character}";
}
