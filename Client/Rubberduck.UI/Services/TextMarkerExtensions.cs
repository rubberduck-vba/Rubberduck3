using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Model;
using Rubberduck.UI.Command.SharedHandlers;
using Rubberduck.UI.Services.Abstract;
using Rubberduck.UI.Shell;
using Rubberduck.UI.Shell.Document;
using System;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
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

    public static void WithTextMarker(this Diagnostic diagnostic, BindableTextEditor editor, TextMarkerService service, ICommand showSettingsCommand)
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

            marker.ToolTip = CreateToolTip(editor, diagnostic, showSettingsCommand);
        }
    }

    private static Popup CreateToolTip(BindableTextEditor editor, Diagnostic diagnostic, ICommand showSettingsCommand)
    {
        var vm = GetToolTipViewModel(diagnostic, showSettingsCommand);
        var tooltip = new TextMarkerToolTip
        {
            DataContext = vm,
            PlacementTarget = editor
        };
        return tooltip;
    }

    private static ITextMarkerToolTip GetToolTipViewModel(Diagnostic diagnostic, ICommand showSettingsCommand)
    {
        var code = diagnostic.Code!.Value.String!;
        var id = (RubberduckDiagnosticId)Convert.ToInt32(code.Substring(3));

        var title = id.ToString(); // TODO get from resources
        var tla = code.Substring(0, 3);
        var type = tla switch
        {
            "VBC" => "VB Compile Error",
            "VBR" => "VB Runtime Error",
            "RD3" => "Rubberduck Diagnostic",
            _ => null,
        };
        return new TextMarkerToolTipViewModel
        {
            Code = code,
            Title = title is null ? code : $"[{code}] {title}",
            Text = diagnostic.Message,
            Type = type ?? "unknown",
            Severity = diagnostic.Severity ?? 0,
            SettingKey = code,
            HelpUri = diagnostic.CodeDescription?.Href,
            ShowSettingsCommand = showSettingsCommand,
        };
    }
}