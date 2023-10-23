using Rubberduck.SettingsProvider.Model;

namespace Rubberduck.Core.Settings
{
    public class UpdateServerSettingsViewModel : ISettingsViewModel<UpdateServerSettingGroup>
    {
        private readonly UpdateServerSettingGroup _settings;

        public UpdateServerSettingsViewModel()
            : this(UpdateServerSettingGroup.Default) { }

        public UpdateServerSettingsViewModel(UpdateServerSettingGroup settings)
        {
            _settings= settings;

            CheckVersionOnStartup = settings.IsEnabled;
            IncludePreReleases = settings.IncludePreReleases;
            RubberduckWebApiBaseUrl = settings.RubberduckWebApiBaseUrl;
            Path = settings.ServerExecutablePath;
        }

        public bool CheckVersionOnStartup { get; set; }
        public bool IncludePreReleases { get; set; }
        public string RubberduckWebApiBaseUrl { get; set; }

        public string Path { get; set; }

        public UpdateServerSettingGroup ToSettings()
        {
            return new UpdateServerSettingGroup
            {
                IsEnabled = this.CheckVersionOnStartup,
                IncludePreReleases = this.IncludePreReleases,
                ServerExecutablePath = this.Path,
                RubberduckWebApiBaseUrl = this.RubberduckWebApiBaseUrl,
            };
        }
    }
}
