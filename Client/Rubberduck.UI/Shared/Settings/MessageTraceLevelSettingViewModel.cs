using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.UI.Shared.Settings.Abstract;

namespace Rubberduck.UI.Shared.Settings
{
    public class MessageTraceLevelSettingViewModel : EnumValueSettingViewModel<MessageTraceLevel>
    {
        public MessageTraceLevelSettingViewModel(TypedRubberduckSetting<MessageTraceLevel> setting) : base(setting)
        {
        }
    }
}
