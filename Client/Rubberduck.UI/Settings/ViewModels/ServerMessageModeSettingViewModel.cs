using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.UI.Settings.ViewModels.Abstract;

namespace Rubberduck.UI.Settings.ViewModels
{
    public class ServerMessageModeSettingViewModel : EnumValueSettingViewModel<MessageMode>
    {
        public ServerMessageModeSettingViewModel(TypedRubberduckSetting<MessageMode> setting) : base(setting)
        {
        }
    }
}
