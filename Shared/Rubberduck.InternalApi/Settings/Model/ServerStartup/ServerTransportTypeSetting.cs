using Rubberduck.InternalApi.ServerPlatform;

namespace Rubberduck.InternalApi.Settings.Model.ServerStartup;

/// <summary>
/// The type of communication channel between this server and its client.
/// </summary>
public record class ServerTransportTypeSetting : TypedRubberduckSetting<TransportType>
{
    public static TransportType DefaultSettingValue { get; } = TransportType.StdIO;

    public ServerTransportTypeSetting()
    {
        SettingDataType = SettingDataType.EnumValueSetting;
        DefaultValue = DefaultSettingValue;
        Tags = SettingTags.ReadOnlyRecommended | SettingTags.Advanced;
    }
}
