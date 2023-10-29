using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model;

namespace Rubberduck.SettingsProvider
{
    public class DefaultUpdateServerSettingsProvider : IDefaultSettingsProvider<UpdateServerSettings>
    {
        public UpdateServerSettings Default { get; } = UpdateServerSettings.Default;
    }
}
