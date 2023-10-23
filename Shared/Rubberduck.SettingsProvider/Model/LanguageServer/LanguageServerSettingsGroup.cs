using Rubberduck.InternalApi.Settings;
using System;
using System.Collections.Generic;

namespace Rubberduck.SettingsProvider.Model.LanguageServer
{
    public interface ILanguageServerSettings
    {
        // TODO add server-side settings here
    }

    public record class LanguageServerSettingsGroup : SettingGroup, IDefaultSettingsProvider<LanguageServerSettingsGroup>, ILanguageServerSettings
    {
        // TODO localize
        private static readonly string _description = "Configures LSP (Language Server Protocol) server options.";

        public LanguageServerSettingsGroup() 
            : base(nameof(LanguageServerSettingsGroup), _description)
        {
        }


        // TODO set defaults here
        protected override IEnumerable<RubberduckSetting> Settings { get; init; } = Array.Empty<RubberduckSetting>();

        public static LanguageServerSettingsGroup Default { get; } = new();
        LanguageServerSettingsGroup IDefaultSettingsProvider<LanguageServerSettingsGroup>.Default => Default;
    }
}
