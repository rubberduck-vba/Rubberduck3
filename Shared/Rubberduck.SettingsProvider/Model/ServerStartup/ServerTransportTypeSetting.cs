using Rubberduck.InternalApi.ServerPlatform;

namespace Rubberduck.SettingsProvider.Model.ServerStartup
{
    public record class ServerTransportTypeSetting : RubberduckSetting<TransportType>
    {
        // TODO localize
        private static readonly string _description = "The type of communication channel between this server and its client.";

        public ServerTransportTypeSetting(string name, TransportType defaultValue)
            : this(name, defaultValue, defaultValue) { }

        public ServerTransportTypeSetting(string name, TransportType defaultValue, TransportType value)
            : base(SettingDataType.EnumSetting, name, _description, defaultValue, value, readOnlyRecommended: true)
        {
        }
    }
}
