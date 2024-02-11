using Rubberduck.InternalApi.Settings.Model;
using Rubberduck.InternalApi.Settings.Model.Editor.Tools;
using Rubberduck.UI.Shared.Settings.Abstract;

namespace Rubberduck.UI.Shared.Settings
{
    public class DockingLocationSettingViewModel : EnumValueSettingViewModel<DockingLocation>
    {
        public DockingLocationSettingViewModel(TypedRubberduckSetting<DockingLocation> setting) : base(setting)
        {
        }
    }
}
