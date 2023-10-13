using Rubberduck.InternalApi.ServerPlatform;
using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model;

namespace Rubberduck.Core.Settings
{
    public class LanguageServerSettingsViewModel : ISettingsViewModel<LanguageServerSettings>
    {
        public LanguageServerSettingsViewModel()
            : this(LanguageServerSettings.Default) { }

        public LanguageServerSettingsViewModel(LanguageServerSettings settings)
        {
            Path = settings.Path;
            TransportType = settings.TransportType;
            PipeName = settings.PipeName;
            Mode = settings.Mode;
            TraceLevel = settings.TraceLevel;
        }

        public string Path { get; set; }

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
}
