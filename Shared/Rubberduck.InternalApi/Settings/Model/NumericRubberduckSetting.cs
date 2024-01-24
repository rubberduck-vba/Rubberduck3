using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.Settings.Model
{
    public record class NumericRubberduckSetting : TypedRubberduckSetting<double>
    {
        public NumericRubberduckSetting()
        {
            SettingDataType = SettingDataType.NumericSetting;
        }

        [JsonIgnore]
        public bool AllowNegative { get; init; } = false;
        [JsonIgnore]
        public bool AllowDecimals { get; init; } = true;
        [JsonIgnore]
        public double MinValue { get; init; } = 0;
        [JsonIgnore]
        public double MaxValue { get; init; } = 1;
    }
}
