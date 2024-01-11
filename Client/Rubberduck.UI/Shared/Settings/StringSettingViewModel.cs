using Rubberduck.SettingsProvider.Model;
using Rubberduck.UI.Shared.Settings.Abstract;

namespace Rubberduck.UI.Shared.Settings
{
    public class StringSettingViewModel : SettingViewModel<string>
    {
        public StringSettingViewModel(StringRubberduckSetting setting) : base(setting)
        {
        }
    }
}
