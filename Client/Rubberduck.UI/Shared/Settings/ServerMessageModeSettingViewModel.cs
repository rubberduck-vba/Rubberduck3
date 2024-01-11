using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.UI.Shared.Settings.Abstract;

namespace Rubberduck.UI.Shared.Settings
{
    public class ServerMessageModeSettingViewModel : EnumValueSettingViewModel<MessageMode>
    {
        public ServerMessageModeSettingViewModel(TypedRubberduckSetting<MessageMode> setting) : base(setting)
        {
        }
    }
}
