using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Model;
using Rubberduck.UI.Extensions;
using Rubberduck.UI.Services.Abstract;
using Rubberduck.UI.Shell;
using Rubberduck.UI.Shell.Document;
using Rubberduck.Unmanaged.Model;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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

public static class DiagnosticExtensions
{
    public static void AddTextMarker(this Diagnostic diagnostic, BindableTextEditor editor, TextMarkerService service)
    {
        var document = editor.Document;
        var start = document.GetOffset(diagnostic.Range.Start.Line, diagnostic.Range.Start.Character);
        var end = document.GetOffset(diagnostic.Range.End.Line, diagnostic.Range.End.Character);
        if (start == end)
        {
            start -= 1;
        }
        var length = end - start + 1;
        var marker = service.Create(start, length);
        if (marker != null)
        {
            marker.MarkerTypes = diagnostic.Severity switch
            {
                DiagnosticSeverity.Hint => TextMarkerTypes.DottedUnderline,
                DiagnosticSeverity.Information => TextMarkerTypes.SquigglyUnderline,
                DiagnosticSeverity.Warning => TextMarkerTypes.SquigglyUnderline,
                DiagnosticSeverity.Error => TextMarkerTypes.SquigglyUnderline,
                _ => TextMarkerTypes.NormalUnderline,
            };
            marker.MarkerColor = diagnostic.Severity switch
            {
                DiagnosticSeverity.Hint => Color.FromRgb(72, 72, 72),
                DiagnosticSeverity.Information => Color.FromRgb(18, 36, 168),
                DiagnosticSeverity.Warning => Color.FromRgb(18, 168, 18),
                DiagnosticSeverity.Error => Color.FromRgb(168, 18, 18),
                _ => Color.FromRgb(0, 0, 0)
            };
            marker.ToolTip = CreateTooltip(editor, diagnostic);
        }
    }

    private static ToolTip CreateTooltip(BindableTextEditor editor, Diagnostic diagnostic)
    {
        var vm = new { TipTitle = diagnostic.Code!.Value.String, TipText = diagnostic.Message, IsError = false, IsInsight = true };
        var tooltip = new TextMarkerToolTip
        {
            DataContext = vm,
            PlacementTarget = editor
        };
        return tooltip;
    }

    private static ToolTip CreateToolTip(BindableTextEditor editor, SyntaxErrorInfo error)
    {
        var vm = new { TipTitle = error.Uri, TipText = error.Message, IsError = true, IsInsight = false };
        var tooltip = new TextMarkerToolTip
        {
            DataContext = vm,
            PlacementTarget = editor
        };
        return tooltip;
    }
}