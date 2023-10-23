using System;

namespace Rubberduck.SettingsProvider.Model.ServerStartup
{
    public record class ClientHealthCheckIntervalSetting : RubberduckSetting<TimeSpan>
    {
        // TODO localize
        private static readonly string _description = "The amount of time between periodic server-side verifications of client connection.";

        public ClientHealthCheckIntervalSetting(string name, TimeSpan defaultValue)
            : this(name, defaultValue, defaultValue) { }

        public ClientHealthCheckIntervalSetting(string name, TimeSpan defaultValue, TimeSpan value) 
            : base(SettingDataType.TimeSpanSetting, name, _description, defaultValue, value, readOnlyRecommended: true)
        {
        }
    }
}
