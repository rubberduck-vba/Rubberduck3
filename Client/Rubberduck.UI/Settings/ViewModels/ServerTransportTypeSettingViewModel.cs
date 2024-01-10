using Rubberduck.SettingsProvider.Model;
using Rubberduck.SettingsProvider.Model.ServerStartup;
using Rubberduck.UI.Settings.ViewModels.Abstract;

namespace Rubberduck.UI.Settings.ViewModels
{
    public class ServerTransportTypeSettingViewModel : EnumValueSettingViewModel<TransportType>
    {
        public ServerTransportTypeSettingViewModel(TypedRubberduckSetting<TransportType> setting) : base(setting)
        {
        }
    }
}
