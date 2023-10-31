using System;

namespace Rubberduck.SettingsProvider.Model.ServerStartup
{
    public class ClientHealthCheckIntervalSetting : TypedRubberduckSetting<TimeSpan>
    {
        // TODO localize
        private static readonly string _description = "The amount of time between periodic server-side verifications of client connection.";
        public static TimeSpan DefaultSettingValue { get; } = TimeSpan.FromSeconds(10);

        public ClientHealthCheckIntervalSetting()
        {
            SettingDataType = SettingDataType.TimeSpanSetting;
            DefaultValue = DefaultSettingValue;
            Tags = SettingTags.ReadOnlyRecommended;
        }
    }
}
