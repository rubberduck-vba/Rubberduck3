using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Rubberduck.InternalApi.ServerPlatform.LanguageServer;

public record class DocumentState
{
    public DocumentState(WorkspaceFileUri uri, string text, int version = 0, bool isOpened = false)
    {
        Uri = uri;
        Text = text;
        Version = version;
        IsOpened = isOpened;
    }

    public WorkspaceFileUri Uri { get; init; }
    public string Text { get; init; }
    public int Version { get; init; }
    public bool IsOpened { get; init; }

    public IImmutableSet<FoldingRange> Foldings { get; init; } = [];
    public IImmutableSet<Diagnostic> Diagnostics { get; init; } = [];
    public Symbol? Symbols { get; init; }

    public DocumentState WithUri(WorkspaceFileUri uri) => this with { Uri = uri };
    public DocumentState WithText(string text) => this with { Text = text, Version = this.Version + 1 };
    public DocumentState WithOpened(bool opened = true) => this with { IsOpened = opened };
    public DocumentState WithFoldings(IEnumerable<FoldingRange> foldings) => this with { Foldings = foldings.ToImmutableHashSet() };
    public DocumentState WithSymbols(Symbol module) => this with { Symbols = module };
    public DocumentState WithDiagnostics(IEnumerable<Diagnostic> diagnostics) => this with { Diagnostics = diagnostics.ToImmutableHashSet() };
}