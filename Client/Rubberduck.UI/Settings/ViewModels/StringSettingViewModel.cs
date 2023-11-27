using Rubberduck.SettingsProvider.Model;
using Rubberduck.UI.Settings.ViewModels.Abstract;

namespace Rubberduck.UI.Settings.ViewModels
{
    public class StringSettingViewModel : SettingViewModel<string>
    {
        public StringSettingViewModel(StringRubberduckSetting setting) : base(setting)
        {
        }
    }
}
