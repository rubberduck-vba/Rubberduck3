using Rubberduck.SettingsProvider.Model;
using Rubberduck.SettingsProvider.Model.ServerStartup;
using Rubberduck.UI.Shared.Settings.Abstract;

namespace Rubberduck.UI.Shared.Settings
{
    public class ServerTransportTypeSettingViewModel : EnumValueSettingViewModel<TransportType>
    {
        public ServerTransportTypeSettingViewModel(TypedRubberduckSetting<TransportType> setting) : base(setting)
        {
        }
    }
}
