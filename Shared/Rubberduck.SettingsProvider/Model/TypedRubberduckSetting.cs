using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rubberduck.SettingsProvider.Model
{
    /// <summary>
    /// Non-generic interface for generic type constraints.
    /// </summary>
    public record class RubberduckSetting 
    {
        /// <summary>
        /// The resource key for this setting.
        /// </summary>
        public string Key { get; init; }
        /// <summary>
        /// The current value of this setting.
        /// </summary>
        public virtual object Value { get; init; }
        /// <summary>
        /// The supported data type of the setting value.
        /// </summary>
        [JsonIgnore]
        public SettingDataType SettingDataType { get; init; }
        /// <summary>
        /// Whether this setting is probably better left alone, but can still be changed if the user knows what they're doing.
        /// </summary>
        [JsonIgnore]
        public bool ReadOnlyRecommended { get; init; }
    }

    public abstract record class TypedRubberduckSetting<TValue> : RubberduckSetting
    {
        /// <summary>
        /// Default constructor for deserialization.
        /// </summary>
        protected TypedRubberduckSetting() { }

        protected TypedRubberduckSetting(string key, TValue? value, SettingDataType settingDataType, TValue defaultValue, bool readOnlyRecommended = false)
        {
            Key = key;
            TypedValue = value ?? defaultValue;
            SettingDataType = settingDataType;
            DefaultValue = defaultValue;
            ReadOnlyRecommended = readOnlyRecommended;
        }
        /// <summary>
        /// The current value/configuration of this setting.
        /// </summary>
        public TValue TypedValue { get; init; }

        /// <summary>
        /// The value returned by <c>Value</c> absent any initialization.
        /// </summary>
        [JsonIgnore]
        public TValue DefaultValue { get; init; }

        public override object Value { get => TypedValue!; init => TypedValue = JsonSerializer.Deserialize<TValue>(value.ToString())!; }
    }
}
