using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model;

namespace Rubberduck.SettingsProvider
{
    public class DefaultUpdateServerSettingsProvider : IDefaultSettingsProvider<UpdateServerSettingGroup>
    {
        public UpdateServerSettingGroup Default { get; } = UpdateServerSettingGroup.Default;
    }
}
