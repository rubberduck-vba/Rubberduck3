using Rubberduck.Core.WebApi;
using Rubberduck.InternalApi.ServerPlatform;
using Rubberduck.SettingsProvider.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubberduck.Core.Settings
{
    public interface ISettingsViewModel<TSettings>
    {
        TSettings ToSettings();
    }

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

    public class FeatureSwitchViewModel : ISettingsViewModel<FeatureSwitch>
    {
        private readonly FeatureSwitch _settings;

        public FeatureSwitchViewModel(FeatureSwitch settings)
        {
            _settings = settings;

            NameResourceKey = settings.NameResourceKey;
            IsEnabled = settings.IsEnabled;
        }

        public string NameResourceKey { get; set; }
        public bool IsEnabled { get; set; }

        public FeatureSwitch ToSettings()
        {
            return new FeatureSwitch
            {
                NameResourceKey = this.NameResourceKey,
                IsEnabled = this.IsEnabled,
            };
        }
    }

    public class LanguageServerSettingsViewModel : ISettingsViewModel<LanguageServerSettings>
    {
        private readonly LanguageServerSettings _settings;

        public LanguageServerSettingsViewModel()
            : this(LanguageServerSettings.Default) { }

        public LanguageServerSettingsViewModel(LanguageServerSettings settings)
        {
            _settings= settings;

            Path = settings.Path;
            TransportType = settings.TransportType;
            PipeName = settings.PipeName;
            Mode = settings.Mode;
            TraceLevel = settings.TraceLevel;
        }

        public string Path { get; set }

        public TransportType TransportType { get; set; }
        public string PipeName { get; set; }
        public MessageMode Mode { get; set; }

        public ServerTraceLevel TraceLevel { get; set; }

        public LanguageServerSettings ToSettings()
        {
            return new LanguageServerSettings
            {
                Mode = this.Mode,
                Path = this.Path,
                PipeName = this.PipeName,
                TraceLevel = this.TraceLevel,
                TransportType = this.TransportType,
            };
        }
    }

    public class UpdateServerSettingsViewModel : ISettingsViewModel<UpdateServerSettings>
    {
        private readonly UpdateServerSettings _settings;

        public UpdateServerSettingsViewModel()
            : this(UpdateServerSettings.Default) { }

        public UpdateServerSettingsViewModel(UpdateServerSettings settings)
        {
            _settings= settings;

            CheckVersionOnStartup = settings.CheckVersionOnStartup;
            IncludePreReleases = settings.IncludePreReleases;
            RubberduckWebApiBaseUrl = settings.RubberduckWebApiBaseUrl;
            Path = settings.Path;
        }

        public bool CheckVersionOnStartup { get; set; }
        public bool IncludePreReleases { get; set; }
        public string RubberduckWebApiBaseUrl { get; set; }

        public string Path { get; set; }

        public UpdateServerSettings ToSettings()
        {
            return new UpdateServerSettings
            {
                CheckVersionOnStartup = this.CheckVersionOnStartup,
                IncludePreReleases = this.IncludePreReleases,
                Path = this.Path,
                RubberduckWebApiBaseUrl = this.RubberduckWebApiBaseUrl,
            };
        }
    }
}
