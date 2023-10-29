using Rubberduck.InternalApi.ServerPlatform;

namespace Rubberduck.SettingsProvider.Model.ServerStartup
{
    public record class ServerTransportTypeSetting : TypedRubberduckSetting<TransportType>
    {
        // TODO localize
        private static readonly string _description = "The type of communication channel between this server and its client.";

        public ServerTransportTypeSetting(string name, TransportType defaultValue)
            : this(name, defaultValue, defaultValue) { }

        public ServerTransportTypeSetting(string name, TransportType defaultValue, TransportType value)
            : base(name, value, SettingDataType.EnumSetting, defaultValue, readOnlyRecommended: true) { }
    }
}
