using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Model;
using Rubberduck.UI.Services.Abstract;
using Rubberduck.UI.Shell;
using Rubberduck.UI.Shell.Document;
using System;
using System.Windows.Controls;
using System.Windows.Media;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Rubberduck.UI.Services;

public static class TextMarkerExtensions
{
    public static void WithTextMarker(this Diagnostic diagnostic, BindableTextEditor editor, TextMarkerService service)
    {
        var document = editor.Document;
        var start = document.GetOffset(diagnostic.Range.Start.Line, diagnostic.Range.Start.Character);
        var end = document.GetOffset(diagnostic.Range.End.Line, diagnostic.Range.End.Character);
 
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
                DiagnosticSeverity.Hint => (TextMarkerTypes.DottedUnderline, Color.FromRgb(72, 72, 72)),
                DiagnosticSeverity.Information => (TextMarkerTypes.SquigglyUnderline, Color.FromRgb(18, 36, 168)),
                DiagnosticSeverity.Warning => (TextMarkerTypes.SquigglyUnderline, Color.FromRgb(18, 168, 18)),
                DiagnosticSeverity.Error => (TextMarkerTypes.SquigglyUnderline, Color.FromRgb(168, 18, 18)),
                _ => (TextMarkerTypes.NormalUnderline, Color.FromRgb(0, 0, 0)),
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