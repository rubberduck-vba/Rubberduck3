using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.UI.Settings.ViewModels.Abstract;

namespace Rubberduck.UI.Settings.ViewModels
{
    public class MessageTraceLevelSettingViewModel : EnumValueSettingViewModel<MessageTraceLevel>
    {
        public MessageTraceLevelSettingViewModel(TypedRubberduckSetting<MessageTraceLevel> setting) : base(setting)
        {
        }
    }
}
