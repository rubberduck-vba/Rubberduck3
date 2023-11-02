using Rubberduck.InternalApi.Settings;

namespace Rubberduck.SettingsProvider.Model.ServerStartup
{
    /// <summary>
    /// The server trace verbosity level.
    /// </summary>
    public record class TraceLevelSetting : TypedRubberduckSetting<MessageTraceLevel>
    {
        public static MessageTraceLevel DefaultSettingValue { get; } = MessageTraceLevel.Verbose;

        public TraceLevelSetting()
        {
            SettingDataType = SettingDataType.EnumValueSetting;
            DefaultValue = DefaultSettingValue;
        }
    }
}
