namespace Rubberduck.InternalApi.Settings.Model.TelemetryServer;

/// <summary>
/// The maximum number of telemetry events transmission payload.
/// </summary>
public record class TelemetryEventQueueSizeSetting : NumericRubberduckSetting
{
    public static double DefaultSettingValue { get; } = 10000;

    public TelemetryEventQueueSizeSetting()
    {
        SettingDataType = SettingDataType.NumericSetting;
        DefaultValue = DefaultSettingValue;
        AllowDecimals = false;
        AllowNegative = false;
        MinValue = 0;
        MaxValue = short.MaxValue;
    }
}
