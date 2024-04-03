using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.Settings.Model.LanguageServer.Diagnostics;

public record class DiagnosticsSettings : EnumSettingGroup<RubberduckDiagnosticId, DiagnosticSetting>
{
    public static DiagnosticSetting[] DefaultSettings =>
        Enum.GetValues<RubberduckDiagnosticId>().Except(Overrides.Keys)
            .Select(e => new DiagnosticSetting { Key = e.Code(), Value = DiagnosticSetting.Default })
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
        DefaultValue = DefaultSettings;
        Value = DefaultValue;
    }

    public DiagnosticSetting(TypedSettingGroup original) : base(original)
    {
    }

    [JsonIgnore]
    public DiagnosticSeverity Severity => GetSetting<DiagnosticSeveritySetting>()?.TypedValue ?? DiagnosticSeveritySetting.DefaultSettingValue;
    public static DiagnosticSetting Default { get; } = new DiagnosticSetting() { Value = DefaultSettings, DefaultValue = DefaultSettings };
}

public record class DiagnosticSeveritySetting : TypedRubberduckSetting<DiagnosticSeverity>
{
    public static DiagnosticSeverity DefaultSettingValue { get; } = DiagnosticSeverity.Warning;
    public DiagnosticSeveritySetting()
    {
        SettingDataType = SettingDataType.EnumValueSetting;
        DefaultValue = DefaultSettingValue;
        Value = DefaultValue;
    }

    public DiagnosticSeveritySetting(DiagnosticSeverity severity)
    {
        SettingDataType = SettingDataType.EnumValueSetting;
        DefaultValue = DefaultSettingValue;
        Value = severity;
    }
}

public record class UseMeaningfulNamesDiagnosticSettings : DiagnosticSetting
{
    private static readonly RubberduckSetting[] DefaultSettings =
        [
            new DiagnosticSeveritySetting(),
            new IgnoredIdentifiersSetting(),
            new MinimumNameLengthSetting(),
            new FlagDisemvoweledNamesSetting(),
            new FlagNumericSuffixesSetting(),
        ];

    public UseMeaningfulNamesDiagnosticSettings()
    {
        Key = RubberduckDiagnosticId.UseMeaningfulIdentifierNames.Code();
        Value = DefaultValue = DefaultSettings;
    }

    [JsonIgnore]
    public string[] IgnoredIdentifierPrefixes => GetSetting<IgnoredIdentifierPrefixesSetting>()?.TypedValue ?? IgnoredIdentifierPrefixesSetting.DefaultSettingValue;

    public new static UseMeaningfulNamesDiagnosticSettings Default { get; } = new UseMeaningfulNamesDiagnosticSettings() { Value = DefaultSettings, DefaultValue = DefaultSettings };
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

public record class IgnoredIdentifiersSetting : TypedRubberduckSetting<string[]>
{
    public static string[] DefaultSettingValue { get; } = [];

    public IgnoredIdentifiersSetting()
    {
        SettingDataType = SettingDataType.ListSetting;
        Value = DefaultValue = DefaultSettingValue;
    }
}

public record class MinimumNameLengthSetting : NumericRubberduckSetting
{
    public static double DefaultSettingValue { get; } = 3;

    public MinimumNameLengthSetting()
    {
        Value = DefaultValue = DefaultSettingValue;
    }
}

public record class FlagDisemvoweledNamesSetting : BooleanRubberduckSetting
{
    public static bool DefaultSettingValue { get; } = true;

    public FlagDisemvoweledNamesSetting()
    {
        Value = DefaultValue = DefaultSettingValue;
    }
}

public record class FlagNumericSuffixesSetting : BooleanRubberduckSetting
{
    public static bool DefaultSettingValue { get; } = true;

    public FlagNumericSuffixesSetting()
    {
        Value = DefaultValue = DefaultSettingValue;
    }
}