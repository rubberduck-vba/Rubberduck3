using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Rubberduck.InternalApi.ServerPlatform.LanguageServer;

public record class DocumentState
{
    public static DocumentState MissingFile(WorkspaceFileUri uri) => 
        new(uri, string.Empty, -1, isOpened: false) { IsMissing = true };
    public static DocumentState LoadError(WorkspaceFileUri uri) =>
        new(uri, string.Empty, -1, isOpened: false) { IsLoadError = true };

    public DocumentState(DocumentState original)
    {
        Id = original.Id;
        Uri = original.Uri;
        Text = original.Text;
        Version = original.Version;
        IsOpened = original.IsOpened;

        Language = original.Language;
        SyntaxErrors = original.SyntaxErrors;
        Foldings = original.Foldings;
        Diagnostics = original.Diagnostics;
        Symbol = original.Symbol;
    }

    public DocumentState(WorkspaceFileUri uri, string text, int version = 1, bool isOpened = false)
    {
        Uri = uri;
        Text = text;
        Version = version;
        IsOpened = isOpened;

        Id = new TextDocumentIdentifier(uri.AbsoluteLocation);
        Language = SupportedLanguage.VBA;
    }

    public void Deconstruct(out WorkspaceFileUri uri, out string text)
    {
        uri = Uri;
        text = Text;
    }

    public TextDocumentIdentifier Id { get; }
    public WorkspaceFileUri Uri { get; init; }
    public string FileExtension => System.IO.Path.GetExtension(Uri.FileName);
    public string Name => Uri.FileNameWithoutExtension;

    public string Text { get; init; }
    public int Version { get; init; }
    public bool IsMissing { get; init; }
    public bool IsLoadError { get; init; }
    public bool IsOpened { get; init; }

    public bool IsModified => Version != 1;


    public SupportedLanguage Language { get; init; }
    public IImmutableSet<SyntaxErrorInfo> SyntaxErrors { get; init; } = [];
    public IImmutableSet<FoldingRange> Foldings { get; init; } = [];
    public IImmutableSet<Diagnostic> Diagnostics { get; init; } = [];
    public Symbol? Symbol { get; init; }


    public DocumentState WithUri(WorkspaceFileUri uri) => this with { Uri = uri };
    public DocumentState WithText(string text) => this with { Text = text, Version = Version + 1 };
    public DocumentState WithOpened(bool opened = true) => this with { IsOpened = opened };


    public DocumentState WithLanguage(SupportedLanguage language) => this with { Language = language };
    public DocumentState WithSyntaxErrors(IEnumerable<SyntaxErrorInfo> errors) => this with { SyntaxErrors = errors.ToImmutableHashSet() };
    public DocumentState WithFoldings(IEnumerable<FoldingRange> foldings) => this with { Foldings = foldings.ToImmutableHashSet() };
    public DocumentState WithDiagnostics(IEnumerable<Diagnostic> diagnostics) => this with { Diagnostics = diagnostics.ToImmutableHashSet() };
    public DocumentState WithSymbol(Symbol module) => this with { Symbol = module };


    /// <summary>
    /// When a file is saved, resetting its version to 1 removes its 'dirty' marker.
    /// </summary>
    public DocumentState WithResetVersion() => this with { Version = 1 };
}