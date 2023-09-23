using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubberduck.SettingsProvider.Model
{
    public readonly struct RubberduckSettings
    {
        public string Locale { get; init; }
        public bool ShowSplash { get; init; }
        public bool IsSmartIndenterPrompted { get; init; }
        
        public FeatureSwitch[] FeatureSwitches { get; init; }

        public LanguageServerSettings LanguageServerSettings { get; init; }
        public UpdateServerSettings UpdateServerSettings { get; init; }
    }
}
