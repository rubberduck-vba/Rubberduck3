using Rubberduck.SettingsProvider.Model;
using Rubberduck.UI.Settings.ViewModels.Abstract;

namespace Rubberduck.UI.Settings.ViewModels
{
    public class NumericSettingViewModel : SettingViewModel<double>
    {
        public NumericSettingViewModel(NumericRubberduckSetting setting) : base(setting)
        {
        }
    }
}
