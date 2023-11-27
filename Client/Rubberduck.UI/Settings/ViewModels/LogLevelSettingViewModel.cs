using Microsoft.Extensions.Logging;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.UI.Settings.ViewModels.Abstract;

namespace Rubberduck.UI.Settings.ViewModels
{
    public class LogLevelSettingViewModel : EnumValueSettingViewModel<LogLevel>
    {
        public LogLevelSettingViewModel(TypedRubberduckSetting<LogLevel> setting) : base(setting)
        {
        }
    }
}
