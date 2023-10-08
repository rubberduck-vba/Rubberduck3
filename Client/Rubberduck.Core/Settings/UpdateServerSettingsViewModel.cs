using Rubberduck.SettingsProvider.Model;

namespace Rubberduck.Core.Settings
{
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
