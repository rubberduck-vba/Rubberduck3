using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System;

namespace Rubberduck.UI.Shell.Document;

public interface ITextMarkerToolTip
{
    string Code { get; }
    string Text { get; }
    string Title { get; }
    Uri? HelpUri { get; }

    string? SettingKey { get; }
    DiagnosticSeverity Severity { get; }
}

internal class TextMarkerToolTipViewModel : ITextMarkerToolTip
{
    public string Code { get; init; }
    public string Text { get; init; }
    public string Title { get; init; }
    public string? SettingKey { get; init; }
    public DiagnosticSeverity Severity { get; init; }
    public Uri? HelpUri { get; init; }
}