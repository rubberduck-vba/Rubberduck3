using Rubberduck.InternalApi.ServerPlatform;
using System;

namespace Rubberduck.SettingsProvider.Model.ServerStartup
{
    /// <summary>
    /// The type of communication channel between this server and its client.
    /// </summary>
    public class ServerTransportTypeSetting : TypedRubberduckSetting<TransportType>
    {
        public static TransportType DefaultSettingValue { get; } = TransportType.StdIO;

        public ServerTransportTypeSetting()
        {
            SettingDataType = SettingDataType.EnumValueSetting;
            DefaultValue = DefaultSettingValue;
            Tags = SettingTags.ReadOnlyRecommended | SettingTags.Advanced;
        }
    }
}
