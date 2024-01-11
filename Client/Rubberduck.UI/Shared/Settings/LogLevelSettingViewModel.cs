using Microsoft.Extensions.Logging;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.UI.Shared.Settings.Abstract;

namespace Rubberduck.UI.Shared.Settings
{
    public class LogLevelSettingViewModel : EnumValueSettingViewModel<LogLevel>
    {
        public LogLevelSettingViewModel(TypedRubberduckSetting<LogLevel> setting) : base(setting)
        {
        }
    }
}
