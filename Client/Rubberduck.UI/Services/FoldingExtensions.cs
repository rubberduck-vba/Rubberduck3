using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Model;
using Rubberduck.UI.Extensions;
using Rubberduck.Unmanaged.Model;
using System.Windows;

namespace Rubberduck.UI.Services;

public static class FoldingExtensions
{
    public static NewFolding ToNewFolding(this FoldingRange folding, ICSharpCode.AvalonEdit.Document.TextDocument document)
    {
        var startPosition = new Selection(folding.StartLine, folding.StartCharacter!.Value).ToTextEditorSelection(document);
        var endPosition = new Selection(folding.EndLine, folding.EndCharacter!.Value).ToTextEditorSelection(document);

        return new NewFolding()
        {
            Name = folding.CollapsedText,
            IsDefinition = folding.Kind == RubberduckFoldingKind.ScopeFoldingKindName,
            DefaultClosed = folding.Kind == RubberduckFoldingKind.HeaderFoldingKindName || folding.Kind == RubberduckFoldingKind.AttributeFoldingKindName,
            StartOffset = startPosition.start,
            EndOffset = endPosition.start
        };
    }
}
