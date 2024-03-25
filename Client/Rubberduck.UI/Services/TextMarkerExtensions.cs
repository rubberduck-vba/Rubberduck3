using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Model;
using Rubberduck.UI.Services.Abstract;
using Rubberduck.UI.Shell;
using Rubberduck.UI.Shell.Document;
using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace Rubberduck.UI.Services;

public static class TextMarkerExtensions
{
    // TODO make these configurable (theming)
    public static readonly Color HintMarkerColor = Color.FromRgb(72, 72, 72);
    public static readonly Color InformationMarkerColor = Color.FromRgb(18, 36, 168);
    public static readonly Color WarningMarkerColor = Color.FromRgb(18, 168, 18);
    public static readonly Color ErrorMarkerColor = Color.FromRgb(168, 18, 18);
    public static readonly Color UndefinedMarkerColor = Color.FromRgb(0, 0, 0);

    /// <summary>
    /// From AvalonEdit.Utils extensions.
    /// </summary>
    public static double CoerceValue(this double value, double minimum, double maximum)
    {
        return Math.Max(Math.Min(value, maximum), minimum);
    }

    public static void WithTextMarker(this Diagnostic diagnostic, BindableTextEditor editor, TextMarkerService service)
    {
        var document = editor.Document;
        var start = document.GetOffset(diagnostic.Range.Start.Line, diagnostic.Range.Start.Character + 1);
        var end = document.GetOffset(diagnostic.Range.End.Line, diagnostic.Range.End.Character + 1);
 
        if (start == end)
        {
            start -= 1;
        }
        var length = end - start;
        var marker = service.Create(start, length);

        if (marker != null)
        {
            (marker.MarkerTypes, marker.MarkerColor) = diagnostic.Severity switch
            {
                DiagnosticSeverity.Hint => (TextMarkerTypes.DottedUnderline, HintMarkerColor),
                DiagnosticSeverity.Information => (TextMarkerTypes.SquigglyUnderline, InformationMarkerColor),
                DiagnosticSeverity.Warning => (TextMarkerTypes.SquigglyUnderline, WarningMarkerColor),
                DiagnosticSeverity.Error => (TextMarkerTypes.SquigglyUnderline, ErrorMarkerColor),
                _ => (TextMarkerTypes.NormalUnderline, UndefinedMarkerColor),
            };

            marker.ToolTip = CreateToolTip(editor, diagnostic);
        }
    }

    private static ToolTip CreateToolTip(BindableTextEditor editor, Diagnostic diagnostic)
    {
        var vm = GetToolTipViewModel(diagnostic);
        var tooltip = new TextMarkerToolTip
        {
            DataContext = vm,
            PlacementTarget = editor
        };
        return tooltip;
    }

    private static object GetToolTipViewModel(Diagnostic diagnostic)
    {
        var isError = diagnostic.Code?.String == RubberduckDiagnosticId.SyntaxError.Code();
        var id = (RubberduckDiagnosticId)Convert.ToInt32(diagnostic.Code?.String?.Substring(3) ?? "-1");


        return new
        {
            TipTitle = $"{diagnostic.Code!.Value.String} {id}",
            TipText = diagnostic.Message,
            IsError = isError,
            IsDiagnostic = !isError
        };
    }
}