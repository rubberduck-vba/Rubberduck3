using Rubberduck.InternalApi.Settings;
using Rubberduck.InternalApi.Settings.Model;

namespace Rubberduck.InternalApi.Settings.Model.ServerStartup;

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
