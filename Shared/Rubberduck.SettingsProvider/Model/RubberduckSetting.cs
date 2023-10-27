using Newtonsoft.Json.Linq;
using System;
using System.Text.Json.Serialization;

namespace Rubberduck.SettingsProvider.Model
{
    public abstract record class NameValueSetting
    {
        public string Name { get; init; }
        public abstract object GetValue();
    }

    public abstract record class RubberduckSetting : NameValueSetting
    {
        public RubberduckSetting(SettingDataType type, string name, string description, bool readOnlyRecommended = false)
        {
            SettingDataType = type;
            Name = name;
            Description = description;
            ReadOnlyRecommended = readOnlyRecommended;
        }

        [JsonIgnore]
        public SettingDataType SettingDataType { get; init; }
        [JsonIgnore]
        public string Description { get; init; }
        [JsonIgnore]
        public bool ReadOnlyRecommended { get; init; }
    }

    public record class RubberduckSetting<TValue> : RubberduckSetting
    {
        public RubberduckSetting(SettingDataType type, string name, string description, TValue defaultValue)
            : this(type, name, description, defaultValue, defaultValue) { }

        public RubberduckSetting(SettingDataType type, string name, string description, TValue defaultValue, TValue value, bool readOnlyRecommended = false)
            : base(type, name, description, readOnlyRecommended)
        {
            //if (SettingDataTypeMap.TypeMap[type] != typeof(TValue))
            //{
            //    throw new InvalidCastException();
            //}

            DefaultValue = defaultValue;
            Value = value;
        }

        public TValue DefaultValue { get; init; }

        public TValue Value { get; init; }

        public override object GetValue()
        {
            return Value ?? throw new NullReferenceException();
        }

        public override string ToString() => Value?.ToString() ?? string.Empty;
    }
}
