using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model.UpdateServer;

namespace Rubberduck.SettingsProvider
{
    public class DefaultUpdateServerSettingsProvider : IDefaultSettingsProvider<UpdateServerSettings>
    {
        public UpdateServerSettings Default { get; } = UpdateServerSettings.Default;
    }
}
