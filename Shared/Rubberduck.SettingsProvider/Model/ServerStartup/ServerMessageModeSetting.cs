using Rubberduck.InternalApi.Settings;

namespace Rubberduck.SettingsProvider.Model.ServerStartup
{
    public record class ServerMessageModeSetting : RubberduckSetting<MessageMode>
    {
        // TODO localize
        private static readonly string _description = "The serialization mode of the JSON-RPC messages, when transport type uses pipes.";

        public ServerMessageModeSetting(string name, MessageMode defaultValue)
            : this(name, defaultValue, defaultValue) { }

        public ServerMessageModeSetting(string name, MessageMode defaultValue, MessageMode value)
            : base(name, value, SettingDataType.EnumSetting, defaultValue, readOnlyRecommended: true)
        {
        }
    }
}
