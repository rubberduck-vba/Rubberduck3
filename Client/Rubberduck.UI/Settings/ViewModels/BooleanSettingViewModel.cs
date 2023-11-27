using Rubberduck.SettingsProvider.Model;
using Rubberduck.UI.Settings.ViewModels.Abstract;

namespace Rubberduck.UI.Settings.ViewModels
{
    public class BooleanSettingViewModel : SettingViewModel<bool>
    {
        public BooleanSettingViewModel(BooleanRubberduckSetting setting) : base(setting)
        {
        }
    }
}
