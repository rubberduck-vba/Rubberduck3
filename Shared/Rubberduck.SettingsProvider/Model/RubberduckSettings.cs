using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubberduck.SettingsProvider.Model
{
    public readonly struct RubberduckSettings : IEquatable<RubberduckSettings>
    {
        public string Locale { get; init; }
        public bool ShowSplash { get; init; }
        public bool IsSmartIndenterPrompted { get; init; }
        
        public FeatureSwitch[] FeatureSwitches { get; init; }

        public LanguageServerSettings LanguageServerSettings { get; init; }
        public UpdateServerSettings UpdateServerSettings { get; init; }

        public bool Equals(RubberduckSettings other)
        {
            var switches = this.FeatureSwitches;
            return string.Equals(Locale, other.Locale, StringComparison.InvariantCultureIgnoreCase)
                && ShowSplash == other.ShowSplash
                && IsSmartIndenterPrompted == other.IsSmartIndenterPrompted
                && LanguageServerSettings.Equals(other.LanguageServerSettings)
                && UpdateServerSettings.Equals(other.UpdateServerSettings)
                && FeatureSwitches.All(e => other.FeatureSwitches.Contains(e))
                && other.FeatureSwitches.All(e => switches.Contains(e));
        }

        public override bool Equals(object obj)
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
            hash.Add(IsSmartIndenterPrompted);
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
