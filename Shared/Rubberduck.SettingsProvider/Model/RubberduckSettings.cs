using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Rubberduck.SettingsProvider.Model
{
    public readonly struct RubberduckSettings : IDefaultSettingsProvider<RubberduckSettings>, IEquatable<RubberduckSettings>
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
            SkippedMessageKeys = Array.Empty<string>(),
        };

        public string Locale { get; init; }
        public bool ShowSplash { get; init; }
        public bool SetDpiUnaware { get; init; }
        public bool IsSmartIndenterPrompted { get; init; }

        public string[] SkippedMessageKeys { get; init; }
        public LogLevel LogLevel { get; init; }
        public bool IsInitialLogLevelChanged { get; init; }

        public FeatureSwitch[] FeatureSwitches { get; init; }

        public LanguageServerSettings LanguageServerSettings { get; init; }
        public UpdateServerSettings UpdateServerSettings { get; init; }

        RubberduckSettings IDefaultSettingsProvider<RubberduckSettings>.Default => RubberduckSettings.Default;

        public bool Equals(RubberduckSettings other)
        {
            var switches = this.FeatureSwitches;
            return string.Equals(Locale, other.Locale, StringComparison.InvariantCultureIgnoreCase)
                && ShowSplash == other.ShowSplash
                && LogLevel == other.LogLevel
                && SetDpiUnaware == other.SetDpiUnaware
                && IsSmartIndenterPrompted == other.IsSmartIndenterPrompted
                && IsInitialLogLevelChanged == other.IsInitialLogLevelChanged
                && LanguageServerSettings.Equals(other.LanguageServerSettings)
                && UpdateServerSettings.Equals(other.UpdateServerSettings)
                && FeatureSwitches.All(e => other.FeatureSwitches.Contains(e))
                && other.FeatureSwitches.All(e => switches.Contains(e));
        }

        public override bool Equals(object? obj)
        {
            if (obj is null || obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((RubberduckSettings)obj);
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(Locale.ToLowerInvariant());
            hash.Add(ShowSplash);
            hash.Add(LogLevel);
            hash.Add(SetDpiUnaware);
            hash.Add(IsSmartIndenterPrompted);
            hash.Add(IsInitialLogLevelChanged);
            hash.Add(LanguageServerSettings);
            hash.Add(UpdateServerSettings);
            foreach (var item in FeatureSwitches)
            {
                hash.Add(item);
            }
            return hash.GetHashCode();
        }
    }
}
