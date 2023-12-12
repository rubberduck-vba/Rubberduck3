using Rubberduck.SettingsProvider.Model;
using Rubberduck.SettingsProvider.Model.Editor.Tools;
using Rubberduck.UI.Settings.ViewModels.Abstract;

namespace Rubberduck.UI.Settings.ViewModels
{
    public class DefaultToolWindowLocationSettingViewModel : EnumValueSettingViewModel<DockingLocation>
    {
        public DefaultToolWindowLocationSettingViewModel(TypedRubberduckSetting<DockingLocation> setting) : base(setting)
        {
        }
    }
}
