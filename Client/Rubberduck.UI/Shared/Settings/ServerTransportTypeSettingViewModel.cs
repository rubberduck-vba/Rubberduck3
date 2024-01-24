using Rubberduck.InternalApi.Settings.Model;
using Rubberduck.InternalApi.Settings.Model.ServerStartup;
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
