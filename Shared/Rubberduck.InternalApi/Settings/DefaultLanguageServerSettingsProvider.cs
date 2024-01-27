using Rubberduck.InternalApi.Settings.Model.LanguageServer;

namespace Rubberduck.InternalApi.Settings;

public class DefaultLanguageServerSettingsProvider : IDefaultSettingsProvider<LanguageServerSettings>
{
    public LanguageServerSettings Default { get; } = LanguageServerSettings.Default;
}
