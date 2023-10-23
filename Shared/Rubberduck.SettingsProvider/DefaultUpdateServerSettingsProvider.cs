using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model;

namespace Rubberduck.SettingsProvider
{
    public class DefaultUpdateServerSettingsProvider : IDefaultSettingsProvider<UpdateServerSettingsGroup>
    {
        public UpdateServerSettingsGroup Default { get; } = UpdateServerSettingsGroup.Default;
    }
}
