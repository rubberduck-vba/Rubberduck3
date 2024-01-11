using Rubberduck.SettingsProvider.Model;
using Rubberduck.SettingsProvider.Model.Editor.Tools;
using Rubberduck.UI.Shared.Settings.Abstract;

namespace Rubberduck.UI.Shared.Settings
{
    public class DefaultToolWindowLocationSettingViewModel : EnumValueSettingViewModel<DockingLocation>
    {
        public DefaultToolWindowLocationSettingViewModel(TypedRubberduckSetting<DockingLocation> setting) : base(setting)
        {
        }
    }
}
