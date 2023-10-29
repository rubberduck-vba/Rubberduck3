using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model.ServerStartup;
using System.Collections.Generic;
using System.Linq;

namespace Rubberduck.SettingsProvider.Model.LanguageServer
{
    public record class LanguageServerSettings : SettingGroup, IDefaultSettingsProvider<LanguageServerSettings>
    {
        // TODO localize
        private static readonly string _description = "Configures LSP (Language Server Protocol) server options.";
        private static readonly IRubberduckSetting[] DefaultSettings =
            new IRubberduckSetting[]
            {
                new TraceLevelSetting(nameof(TraceLevelSetting), MessageTraceLevel.Verbose),
                new LanguageServerStartupSettings(),
            };

        public LanguageServerSettings() 
            : base(nameof(LanguageServerSettings), DefaultSettings, DefaultSettings) { }

        public LanguageServerSettings(LanguageServerSettings original, IEnumerable<IRubberduckSetting>? settings)
            : base(original)
        {
            Value = settings?.ToArray() ?? DefaultSettings;
        }

        public LanguageServerSettings(params IRubberduckSetting[] settings)
            : base(nameof(LanguageServerSettings), settings, DefaultSettings) { }

        public LanguageServerSettings(IEnumerable<IRubberduckSetting> settings)
            : base(nameof(LanguageServerSettings), settings, DefaultSettings) { }


        public MessageTraceLevel TraceLevel => GetSetting<TraceLevelSetting>().Value;
        public LanguageServerStartupSettings StartupSettings => GetSetting<LanguageServerStartupSettings>();

        public static LanguageServerSettings Default { get; } = new(DefaultSettings);
        LanguageServerSettings IDefaultSettingsProvider<LanguageServerSettings>.Default => Default;
    }
}
