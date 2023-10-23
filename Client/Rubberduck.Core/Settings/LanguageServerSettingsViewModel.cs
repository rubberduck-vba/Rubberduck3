using Rubberduck.InternalApi.ServerPlatform;
using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model.LanguageServer;

namespace Rubberduck.Core.Settings
{
    public class LanguageServerSettingsViewModel : ISettingsViewModel<LanguageServerSettingsGroup>
    {
        public LanguageServerSettingsViewModel()
            : this(LanguageServerSettingsGroup.Default) { }

        public LanguageServerSettingsViewModel(LanguageServerSettingsGroup settings)
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

        public LanguageServerSettingsGroup ToSettings()
        {
            return new LanguageServerSettingsGroup
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
