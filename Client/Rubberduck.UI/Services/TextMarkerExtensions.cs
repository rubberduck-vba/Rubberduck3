using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Model;
using Rubberduck.UI.Services.Abstract;
using Rubberduck.UI.Shell;
using Rubberduck.UI.Shell.Document;
using System.Windows.Controls;
using System.Windows.Media;

namespace Rubberduck.UI.Services;

public static class TextMarkerExtensions
{
    public static void WithTextMarker(this SyntaxErrorInfo error, BindableTextEditor editor, TextMarkerService service)
    {
        var document = editor.Document;
        var start = document.GetOffset(error.Range.Start.Line, error.Range.Start.Character);
        var end = document.GetOffset(error.Range.End.Line, error.Range.End.Character);

        if (start == end)
        {
            start -= 1;
        }
        var length = end - start + 1;

        var marker = service.Create(start, length);
        if (marker is not null)
        {
            marker.MarkerTypes = TextMarkerTypes.SquigglyUnderline;
            marker.MarkerColor = Color.FromRgb(168, 18, 18);

            marker.ToolTip = CreateToolTip(editor, error);
        }
    }

    public static void WithTextMarker(this Diagnostic diagnostic, BindableTextEditor editor, TextMarkerService service)
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

            marker.ToolTip = CreateToolTip(editor, diagnostic);
        }
    }

    private static ToolTip CreateToolTip(BindableTextEditor editor, Diagnostic diagnostic)
    {
        var vm = new { TipTitle = diagnostic.Code!.Value.String, TipText = diagnostic.Message, IsError = false, IsDiagnostic = true };
        var tooltip = new TextMarkerToolTip
        {
            DataContext = vm,
            PlacementTarget = editor
        };
        return tooltip;
    }

    private static ToolTip CreateToolTip(BindableTextEditor editor, SyntaxErrorInfo error)
    {
        var vm = new { TipTitle = error.Uri, TipText = error.Message, IsError = true, IsDiagnostic = false };
        var tooltip = new TextMarkerToolTip
        {
            DataContext = vm,
            PlacementTarget = editor
        };
        return tooltip;
    }
}