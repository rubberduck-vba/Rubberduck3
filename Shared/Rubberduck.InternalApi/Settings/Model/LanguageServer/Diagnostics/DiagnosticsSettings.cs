using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.Settings.Model.LanguageServer.Diagnostics;

public record class DiagnosticsSettings : TypedSettingGroup //<RubberduckDiagnosticId>
{
    public static DiagnosticSetting[] DefaultSettings =>
        Enum.GetValues<RubberduckDiagnosticId>().Except(Overrides.Keys)
            .Select(e => new DiagnosticSetting { Key = e.Code()})
            .Concat(Overrides.Values)
            .ToArray();

    public DiagnosticsSettings()
    {
        DefaultValue = DefaultSettings;
    }

    private static Dictionary<RubberduckDiagnosticId, DiagnosticSetting> Overrides { get; } = new()
    {
        [RubberduckDiagnosticId.UseMeaningfulIdentifierNames] = UseMeaningfulNamesDiagnosticSettings.Default,
    };

    public static DiagnosticsSettings Default { get; } = new DiagnosticsSettings() { Value = DefaultSettings, DefaultValue = DefaultSettings };
}

/// <summary>
/// The base class for configuring an individual diagnostic.
/// </summary>
public record class DiagnosticSetting : TypedSettingGroup
{
    private static readonly RubberduckSetting[] DefaultSettings =
        [
            new DiagnosticSeveritySetting(),
        ];

    public DiagnosticSetting()
    {
        Value = DefaultValue = DefaultSettings;
    }

    [JsonIgnore]
    public DiagnosticSeverity Severity => GetSetting<DiagnosticSeveritySetting>()?.TypedValue ?? DiagnosticSeveritySetting.DefaultSettingValue;
}

public record class DiagnosticSeveritySetting : TypedRubberduckSetting<DiagnosticSeverity>
{
    public static DiagnosticSeverity DefaultSettingValue { get; } = DiagnosticSeverity.Warning;
    public DiagnosticSeveritySetting()
    {
        SettingDataType = SettingDataType.EnumValueSetting;
        Value = DefaultValue = DefaultSettingValue;
    }
}

public record class UseMeaningfulNamesDiagnosticSettings : DiagnosticSetting
{
    private static readonly RubberduckSetting[] DefaultSettings =
        [
            new DiagnosticSeveritySetting(),
            new IgnoredIdentifierPrefixesSetting(),
        ];

    public UseMeaningfulNamesDiagnosticSettings()
    {
        Value = DefaultValue = DefaultSettings;
    }

    [JsonIgnore]
    public string[] IgnoredIdentifierPrefixes => GetSetting<IgnoredIdentifierPrefixesSetting>()?.TypedValue ?? IgnoredIdentifierPrefixesSetting.DefaultSettingValue;

    public static UseMeaningfulNamesDiagnosticSettings Default { get; } = new UseMeaningfulNamesDiagnosticSettings() { Value = DefaultSettings, DefaultValue = DefaultSettings };
}

public record class IgnoredIdentifierPrefixesSetting : TypedRubberduckSetting<string[]>
{
    public static string[] DefaultSettingValue { get; } = [];

    public IgnoredIdentifierPrefixesSetting()
    {
        SettingDataType = SettingDataType.ListSetting;
        Value = DefaultValue = DefaultSettingValue;
    }
}
