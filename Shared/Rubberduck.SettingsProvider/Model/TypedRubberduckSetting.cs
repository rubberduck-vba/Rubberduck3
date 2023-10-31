using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rubberduck.SettingsProvider.Model
{
    public record class StringRubberduckSetting : TypedRubberduckSetting<string>
    {
        public StringRubberduckSetting() 
        {
            SettingDataType = SettingDataType.TextSetting;
        }

        [JsonIgnore]
        public bool IsRequired { get; init; } = true;
        [JsonIgnore]
        public int? MinLength { get; init; } = 1;
        [JsonIgnore]
        public int? MaxLength { get; init; } = short.MaxValue;
        [JsonIgnore]
        public string? RegEx { get; init; }
    }

    public record class BooleanRubberduckSetting : TypedRubberduckSetting<bool>
    {
        public BooleanRubberduckSetting()
        {
            SettingDataType = SettingDataType.BooleanSetting;
        }
    }

    public record class UriRubberduckSetting : TypedRubberduckSetting<Uri>
    {
        public UriRubberduckSetting()
        {
            SettingDataType = SettingDataType.UriSetting;
        }
    }

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

    public record class TypedRubberduckSetting<TValue> : RubberduckSetting
    {
        /// <summary>
        /// The current value/configuration of this setting.
        /// </summary>
        [JsonIgnore]
        public TValue TypedValue { get; private set; }

        /// <summary>
        /// The value returned by <c>Value</c> absent any initialization.
        /// </summary>
        [JsonIgnore]
        public TValue DefaultValue { get; init; }

        [JsonPropertyOrder(1)]
        public override object Value
        {
            get => TypedValue!;
            set
            {
                if (SettingDataType == SettingDataType.SettingGroup)
                {
                    if (value is JsonElement json && json.ValueKind == JsonValueKind.Array)
                    {
                        object values = json.EnumerateArray().Select(item => item.Deserialize<RubberduckSetting>()!).ToArray();
                        TypedValue = (TValue)values;
                    }
                    else
                    {
                        TypedValue = (TValue)value!;
                    }
                }
                else if (SettingDataType == SettingDataType.EnumSettingGroup)
                {
                    if (value is JsonElement json && json.ValueKind == JsonValueKind.Array)
                    {
                        object values = json.EnumerateArray().Select(item => item.Deserialize<BooleanRubberduckSetting>()!).ToArray();
                        TypedValue = (TValue)values;
                    }
                    else
                    {
                        TypedValue = (TValue)value!;
                    }
                }
                else if (SettingDataType != SettingDataType.NotSet)
                {
                    if (value is JsonElement json)
                    {
                        TypedValue = json.Deserialize<TValue>()!;
                    }
                    else
                    {
                        TypedValue = (TValue)value!;
                    }
                }
                else
                {
                    throw new InvalidOperationException($"SettingDataType is unset for setting type '{Key}'.");
                }
            }
        }

        public TypedRubberduckSetting<TValue> WithValue(TValue value)
        {
            _ = value ?? throw new ArgumentNullException(nameof(value));
            Value = value;
            return this;
        }
    }
}
