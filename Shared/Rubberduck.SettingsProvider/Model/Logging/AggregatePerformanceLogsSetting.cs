namespace Rubberduck.SettingsProvider.Model.Logging
{
    /// <summary>
    /// Enables/disables aggregation of performance trace entries, which can get particularly noisy.
    /// </summary>
    public record class AggregatePerformanceLogsSetting : BooleanRubberduckSetting
    {
        public static bool DefaultSettingValue { get; } = true;

        public AggregatePerformanceLogsSetting()
        {
            DefaultValue = DefaultSettingValue;
            Tags = SettingTags.Advanced | SettingTags.ReadOnlyRecommended;
        }
    }

    /// <summary>
    /// Sets the minimum number of low-volume performance records to accumulate before they are aggregated and written into a log entry.
    /// </summary>
    public record class SmallAggregateSampleSizeSetting : NumericRubberduckSetting
    {
        public static double DefaultSettingValue { get; } = 10;

        public SmallAggregateSampleSizeSetting()
        {
            DefaultValue = DefaultSettingValue;
            Tags = SettingTags.Advanced | SettingTags.ReadOnlyRecommended;
        }
    }

    /// <summary>
    /// Sets the minimum number of high-volume performance records to accumulate before they are aggregated and written into a log entry.
    /// </summary>
    public record class LargeAggregateSampleSizeSetting : NumericRubberduckSetting
    {
        public static double DefaultSettingValue { get; } = 100;

        public LargeAggregateSampleSizeSetting()
        {
            DefaultValue = DefaultSettingValue;
            Tags = SettingTags.Advanced | SettingTags.ReadOnlyRecommended;
        }
    }

    /// <summary>
    /// Sets the maximum number of performance records before a record cache is cleared.
    /// </summary>
    public record class MaxAggregateSampleSizeSetting : NumericRubberduckSetting
    {
        public static double DefaultSettingValue { get; } = 1000;

        public MaxAggregateSampleSizeSetting()
        {
            DefaultValue = DefaultSettingValue;
            Tags = SettingTags.Advanced | SettingTags.ReadOnlyRecommended;
        }
    }

    /// <summary>
    /// The maximum number of events that can be recorded in a minute in order to log performance using low-volume thresholds.
    /// </summary>
    public record class LowVolumeEventsPerMinuteSetting : NumericRubberduckSetting
    {
        public static double DefaultSettingValue { get; } = 1;

        public LowVolumeEventsPerMinuteSetting()
        {
            DefaultValue = DefaultSettingValue;
            Tags = SettingTags.Advanced | SettingTags.ReadOnlyRecommended;
        }
    }

    /// <summary>
    /// The maximum number of events that can be recorded in a minute in order to log performance using high-volume thresholds.
    /// </summary>
    /// <remarks>
    /// Higher-volume events will only be recorded at <c>MaxAggregateSampleSize</c>.
    /// </remarks>
    public record class HighVolumeEventsPerMinuteSetting : NumericRubberduckSetting
    {
        public static double DefaultSettingValue { get; } = 6;

        public HighVolumeEventsPerMinuteSetting()
        {
            DefaultValue = DefaultSettingValue;
            Tags = SettingTags.Advanced | SettingTags.ReadOnlyRecommended;
        }
    }
}
