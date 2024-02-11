using Rubberduck.InternalApi.Settings.Model;
using Rubberduck.UI.Shared.Settings.Abstract;

namespace Rubberduck.UI.Shared.Settings
{
    public class BooleanSettingViewModel : SettingViewModel<bool>
    {
        public BooleanSettingViewModel(BooleanRubberduckSetting setting) : base(setting)
        {
        }
    }
}
