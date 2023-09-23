using Rubberduck.Core.WebApi;
using Rubberduck.SettingsProvider.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubberduck.Core.Settings
{
    public class RubberduckSettingsViewModel : ISettingsViewModel<RubberduckSettings>
    {
        private readonly RubberduckSettings _settings;

        public RubberduckSettingsViewModel(RubberduckSettings settings)
        {
            _settings = settings;

            Locale = _settings.Locale;
            ShowSplash = _settings.ShowSplash;
            IsSmartIndenterPrompted = _settings.IsSmartIndenterPrompted;

            FeatureSwitches = _settings.FeatureSwitches.Select(e => new FeatureSwitchViewModel(e)).ToList();
            LanguageServerSettings = new LanguageServerSettingsViewModel(_settings.LanguageServerSettings);
            UpdateServerSettings = new UpdateServerSettingsViewModel(_settings.UpdateServerSettings);
        }

        public string Locale { get; set; }
        public bool ShowSplash { get; set; }
        public bool IsSmartIndenterPrompted { get; set; }

        public List<FeatureSwitchViewModel> FeatureSwitches { get; }

        public LanguageServerSettingsViewModel LanguageServerSettings { get; }
        public UpdateServerSettingsViewModel UpdateServerSettings { get; }

        public RubberduckSettings ToSettings()
        {
            return new RubberduckSettings
            {
                Locale = this.Locale,
                ShowSplash = this.ShowSplash,
                IsSmartIndenterPrompted = this.IsSmartIndenterPrompted,
                FeatureSwitches = this.FeatureSwitches.Select(e => e.ToSettings()).ToArray(),
                LanguageServerSettings = this.LanguageServerSettings.ToSettings(),
                UpdateServerSettings = this.UpdateServerSettings.ToSettings(),
            };
        }
    }
}
