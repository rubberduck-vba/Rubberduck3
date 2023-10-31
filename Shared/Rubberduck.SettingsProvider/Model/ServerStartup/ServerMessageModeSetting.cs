using Rubberduck.InternalApi.Settings;
using System;

namespace Rubberduck.SettingsProvider.Model.ServerStartup
{
    /// <summary>
    /// The serialization mode of the JSON-RPC messages, when transport type uses pipes.
    /// </summary>
    public class ServerMessageModeSetting : TypedRubberduckSetting<MessageMode>
    {
        public static MessageMode DefaultSettingValue { get; } = MessageMode.Message;

        public ServerMessageModeSetting()
        {
            SettingDataType = SettingDataType.EnumValueSetting;
            DefaultValue = DefaultSettingValue;
            Tags = SettingTags.ReadOnlyRecommended | SettingTags.Advanced;
        }
    }
}
