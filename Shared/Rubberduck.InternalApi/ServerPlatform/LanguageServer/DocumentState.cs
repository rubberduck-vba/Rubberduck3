using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Rubberduck.InternalApi.ServerPlatform.LanguageServer;

public record class CodeDocumentState : DocumentState
{
    public CodeDocumentState(CodeDocumentState original) 
        : base(original)
    {
        Language = original.Language;
        Foldings = original.Foldings;
        Diagnostics = original.Diagnostics;
        Symbol = original.Symbol;
    }

    public CodeDocumentState(WorkspaceFileUri uri, SupportedLanguage language, string text, int version = 1, bool isOpened = false) 
        : base(uri, text, version, isOpened)
    {
        Language = language;
    }

    public SupportedLanguage Language { get; init; }
    public IReadOnlyCollection<FoldingRange> Foldings { get; init; } = [];
    public IReadOnlyCollection<Diagnostic> Diagnostics { get; init; } = [];
    public Symbol? Symbol { get; init; }

    public CodeDocumentState WithLanguage(SupportedLanguage language) => this with { Language = language };
    public CodeDocumentState WithFoldings(IEnumerable<FoldingRange> foldings) => this with { Foldings = foldings.ToImmutableHashSet() };
    public CodeDocumentState WithDiagnostics(IEnumerable<Diagnostic> diagnostics) => this with { Diagnostics = diagnostics.ToImmutableHashSet() };
    public CodeDocumentState WithSymbol(Symbol symbol) => this with { Symbol = symbol };
}

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
    }

    public DocumentState(WorkspaceFileUri uri, string text, int version = 1, bool isOpened = false)
    {
        Uri = uri;
        Text = text;
        Version = version;
        IsOpened = isOpened;

        Id = new TextDocumentIdentifier(uri.AbsoluteLocation);
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

    /// <summary>
    /// Gets a copy of this record with the specified <c>Uri</c>.
    /// </summary>
    public DocumentState WithUri(WorkspaceFileUri uri) => this with { Uri = uri };
    /// <summary>
    /// Gets a copy of this record with the specified <c>Text</c> and an incremented <c>Version</c> number.
    /// </summary>
    public DocumentState WithText(string text) => this with { Text = text, Version = Version + 1 };
    /// <summary>
    /// Gets a copy of this record with the <c>IsOpened</c> property as specified.
    /// </summary>
    public DocumentState WithOpened(bool opened = true) => this with { IsOpened = opened };


    /// <summary>
    /// When a file is saved, resetting its version to 1 removes its 'dirty' marker.
    /// </summary>
    public DocumentState WithResetVersion() => this with { Version = 1 };
}