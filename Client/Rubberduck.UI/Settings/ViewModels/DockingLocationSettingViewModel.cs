using Rubberduck.SettingsProvider.Model;
using Rubberduck.SettingsProvider.Model.Editor.Tools;
using Rubberduck.UI.Settings.ViewModels.Abstract;

namespace Rubberduck.UI.Settings.ViewModels
{
    public class DockingLocationSettingViewModel : EnumValueSettingViewModel<DockingLocation>
    {
        public DockingLocationSettingViewModel(TypedRubberduckSetting<DockingLocation> setting) : base(setting)
        {
        }
    }
}
