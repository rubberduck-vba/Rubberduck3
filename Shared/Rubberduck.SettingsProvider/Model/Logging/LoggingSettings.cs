using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model.ServerStartup;
using System.Text.Json.Serialization;

namespace Rubberduck.SettingsProvider.Model.Logging
{
    public record class LoggingSettings : TypedSettingGroup, IDefaultSettingsProvider<LoggingSettings>
    {
        private static readonly RubberduckSetting[] DefaultSettings =
            new RubberduckSetting[]
            {
                new DisableInitialLogLevelResetSetting { Value = DisableInitialLogLevelResetSetting.DefaultSettingValue },
                new LogLevelSetting { Value = LogLevelSetting.DefaultSettingValue },
                new TraceLevelSetting { Value = TraceLevelSetting.DefaultSettingValue },
                new AggregatePerformanceLogsSetting { Value = AggregatePerformanceLogsSetting.DefaultSettingValue },
                new SmallAggregateSampleSizeSetting { Value = SmallAggregateSampleSizeSetting.DefaultSettingValue },
                new LargeAggregateSampleSizeSetting { Value = LargeAggregateSampleSizeSetting.DefaultSettingValue },
                new MaxAggregateSampleSizeSetting { Value = MaxAggregateSampleSizeSetting.DefaultSettingValue },
                new LowVolumeEventsPerMinuteSetting { Value = LowVolumeEventsPerMinuteSetting.DefaultSettingValue },
                new HighVolumeEventsPerMinuteSetting { Value = HighVolumeEventsPerMinuteSetting.DefaultSettingValue },
            };

        public LoggingSettings() { }

        [JsonIgnore]
        public bool DisableInitialLogLevelReset => GetSetting<DisableInitialLogLevelResetSetting>()?.TypedValue ?? DisableInitialLogLevelResetSetting.DefaultSettingValue;
        [JsonIgnore]
        public LogLevel LogLevel => GetSetting<LogLevelSetting>()?.TypedValue ?? LogLevelSetting.DefaultSettingValue;
        [JsonIgnore]
        public MessageTraceLevel TraceLevel => GetSetting<TraceLevelSetting>()?.TypedValue ?? TraceLevelSetting.DefaultSettingValue;
        [JsonIgnore]
        public bool AggregatePerformanceLogs => GetSetting<AggregatePerformanceLogsSetting>()?.TypedValue ?? AggregatePerformanceLogsSetting.DefaultSettingValue;
        [JsonIgnore]
        public double SmallAggregateSampleSize => GetSetting<SmallAggregateSampleSizeSetting>()?.TypedValue ?? SmallAggregateSampleSizeSetting.DefaultSettingValue;
        [JsonIgnore]
        public double LargeAggregateSampleSize => GetSetting<LargeAggregateSampleSizeSetting>()?.TypedValue ?? LargeAggregateSampleSizeSetting.DefaultSettingValue;
        [JsonIgnore]
        public double MaxAggregateSampleSize => GetSetting<MaxAggregateSampleSizeSetting>()?.TypedValue ?? MaxAggregateSampleSizeSetting.DefaultSettingValue;
        [JsonIgnore]
        public double LowVolumeEventsPerMinute => GetSetting<LowVolumeEventsPerMinuteSetting>()?.TypedValue ?? LowVolumeEventsPerMinuteSetting.DefaultSettingValue;
        [JsonIgnore]
        public double HighVolumeEventsPerMinute => GetSetting<HighVolumeEventsPerMinuteSetting>()?.TypedValue ?? HighVolumeEventsPerMinuteSetting.DefaultSettingValue;

        public static LoggingSettings Default { get; } = new LoggingSettings() { Value = DefaultSettings, DefaultValue = DefaultSettings };
        LoggingSettings IDefaultSettingsProvider<LoggingSettings>.Default => Default;
    }
}
