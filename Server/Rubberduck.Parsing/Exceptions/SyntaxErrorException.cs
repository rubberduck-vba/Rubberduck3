using Antlr4.Runtime;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using System.Windows.Shapes;
using System;

namespace Rubberduck.Parsing.Exceptions;

/// <summary>
/// An exception that is thrown when the parser fails in SLL prediction mode, which ultimately gets reported as a diagnostic.
/// </summary>
public class SllPredictionFailException : SyntaxErrorException
{
    public SllPredictionFailException(AntlrSyntaxErrorInfo info) : base(info) { }
    public Diagnostic ToDiagnostic()
    {
        var range = this.Range();
        return RubberduckDiagnostic.SllFailure(new SyntaxErrorOffendingSymbol(OffendingSymbol.Text, Uri)
        {
            IsUserDefined = true,
            SelectionRange = range,
            Range = range,
        });
    }
}

/// <summary>
/// An exception that is thrown when the parser encounters a syntax error.
/// This exception indicates either a bug in the grammar... or non-compilable VBA code.
/// </summary>
[Serializable]
public class SyntaxErrorException : Exception
{
    public SyntaxErrorException(AntlrSyntaxErrorInfo info)
        : base(info.Message, info.Exception)
    {
        Uri = info.Uri;

        OffendingSymbol = info.OffendingSymbol;
        LineNumber = info.LineNumber;
        Position = info.Position;
    }

    public WorkspaceFileUri Uri { get; init; }

    public IToken OffendingSymbol { get; init; }
    public int LineNumber { get; init; }
    public int Position { get; init; }

    public OmniSharp.Extensions.LanguageServer.Protocol.Models.Range Range() =>
    new(start: new Position(OffendingSymbol.Line, OffendingSymbol.Column),
        end: new Position(OffendingSymbol.EndLine(), OffendingSymbol.EndColumn()));

    public override string ToString() => $"{base.ToString()}\nToken: {OffendingSymbol.Text} at L{LineNumber}C{Position}";
}


public record class SyntaxErrorOffendingSymbol : Symbol
{
    public SyntaxErrorOffendingSymbol(string name, WorkspaceUri? parentUri = null) 
        : base(RubberduckSymbolKind.UnknownSymbol, name, parentUri, InternalApi.Model.Accessibility.Undefined, [])
    {
    }
}