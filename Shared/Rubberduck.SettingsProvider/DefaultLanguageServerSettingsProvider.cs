using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model.LanguageServer;

namespace Rubberduck.SettingsProvider
{
    public class DefaultLanguageServerSettingsProvider : IDefaultSettingsProvider<LanguageServerSettingsGroup>
    {
        public LanguageServerSettingsGroup Default { get; } = LanguageServerSettingsGroup.Default;
    }
}
