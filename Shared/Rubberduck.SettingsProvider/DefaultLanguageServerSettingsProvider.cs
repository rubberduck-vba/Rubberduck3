using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model;

namespace Rubberduck.SettingsProvider
{
    public class DefaultLanguageServerSettingsProvider : IDefaultSettingsProvider<LanguageServerSettings>
    {
        public LanguageServerSettings Default { get; } = LanguageServerSettings.Default;
    }
}
