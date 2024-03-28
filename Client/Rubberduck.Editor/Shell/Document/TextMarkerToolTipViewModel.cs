using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.UI;
using Rubberduck.UI.Shell.Document;
using System;

namespace Rubberduck.Editor.Shell.Document;

public class TextMarkerToolTipViewModel : ViewModelBase, ITextMarkerToolTip
{
    public TextMarkerToolTipViewModel(DiagnosticSeverity severity, string code, string title, string text, string? key, Uri? helpUri)
    {
        Code = code;
        Severity = severity;
        Title = title;
        Text = text;
        SettingKey = key;
        HelpUri = helpUri;
    }

    public string Code { get; }

    public string Text { get; }

    public string Title { get; }

    public string? SettingKey { get; }

    public DiagnosticSeverity Severity { get; }

    public Uri? HelpUri { get; }
}
