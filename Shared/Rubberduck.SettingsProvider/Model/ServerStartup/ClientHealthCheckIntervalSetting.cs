using System;

namespace Rubberduck.SettingsProvider.Model.ServerStartup
{
    /// <summary>
    /// The amount of time between periodic server-side verifications of client connection.
    /// </summary>
    public record class ClientHealthCheckIntervalSetting : TypedRubberduckSetting<TimeSpan>
    {
        public static TimeSpan DefaultSettingValue { get; } = TimeSpan.FromSeconds(10);

        public ClientHealthCheckIntervalSetting()
        {
            SettingDataType = SettingDataType.TimeSpanSetting;
            DefaultValue = DefaultSettingValue;
            Tags = SettingTags.ReadOnlyRecommended;
        }
    }
}
