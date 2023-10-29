using System.Text.Json.Serialization;

namespace Rubberduck.SettingsProvider.Model
{
    /// <summary>
    /// Non-generic interface for generic type constraints.
    /// </summary>
    public interface IRubberduckSetting 
    {
        /// <summary>
        /// The resource key for this setting.
        /// </summary>
        public string NameKey { get; init; }
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

    public abstract record class RubberduckSetting<TValue> : IRubberduckSetting
    {
        /// <summary>
        /// Default constructor for deserialization.
        /// </summary>
        protected RubberduckSetting() { }

        protected RubberduckSetting(string nameKey, TValue? value, SettingDataType settingDataType, TValue defaultValue, bool readOnlyRecommended = false)
        {
            NameKey = nameKey;
            Value = value ?? defaultValue;
            SettingDataType = settingDataType;
            DefaultValue = defaultValue;
            ReadOnlyRecommended = readOnlyRecommended;
        }

        /// <summary>
        /// The resource key for this setting.
        /// </summary>
        public string NameKey { get; init; }

        /// <summary>
        /// The current value/configuration of this setting.
        /// </summary>
        public TValue Value { get; init; }

        /// <summary>
        /// The value returned by <c>Value</c> absent any initialization.
        /// </summary>
        [JsonIgnore]
        public TValue DefaultValue { get; init; }

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
}
