using Microsoft.Extensions.Logging;
using System;

namespace Rubberduck.SettingsProvider.Model
{
    public readonly record struct RubberduckSettings : IDefaultSettingsProvider<RubberduckSettings>
    {
        public static RubberduckSettings Default { get; } = new RubberduckSettings
        {
            Locale = "en-US",
            LogLevel = LogLevel.Trace,
            ShowSplash = true,
            SetDpiUnaware = false,
            IsInitialLogLevelChanged = false,
            IsSmartIndenterPrompted = true,
            FeatureSwitches = Array.Empty<FeatureSwitch>(),
            UpdateServerSettings = UpdateServerSettings.Default,
            LanguageServerSettings = LanguageServerSettings.Default,
            LanguageClientSettings = LanguageClientSettings.Default,
        };

        public string Locale { get; init; }
        public bool ShowSplash { get; init; }
        public bool SetDpiUnaware { get; init; }
        public bool IsSmartIndenterPrompted { get; init; }

        public LogLevel LogLevel { get; init; }
        public bool IsInitialLogLevelChanged { get; init; }

        public FeatureSwitch[] FeatureSwitches { get; init; }

        public LanguageClientSettings LanguageClientSettings { get; init; }

        public LanguageServerSettings LanguageServerSettings { get; init; }
        public UpdateServerSettings UpdateServerSettings { get; init; }
        public TelemetryServerSettings TelemetryServerSettings { get; init; }

        RubberduckSettings IDefaultSettingsProvider<RubberduckSettings>.Default => RubberduckSettings.Default;
    }
}
