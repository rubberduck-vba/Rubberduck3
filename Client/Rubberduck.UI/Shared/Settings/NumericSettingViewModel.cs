using Rubberduck.SettingsProvider.Model;
using Rubberduck.UI.Shared.Settings.Abstract;

namespace Rubberduck.UI.Shared.Settings
{
    public class NumericSettingViewModel : SettingViewModel<double>
    {
        public NumericSettingViewModel(NumericRubberduckSetting setting) : base(setting)
        {
        }
    }
}
