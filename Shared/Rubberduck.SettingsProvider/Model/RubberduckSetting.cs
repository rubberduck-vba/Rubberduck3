using Newtonsoft.Json.Linq;
using System;

namespace Rubberduck.SettingsProvider.Model
{
    public abstract record class RubberduckSetting
    {
        public RubberduckSetting(SettingDataType type, string name, string description, bool readOnlyRecommended = false)
        {
            SettingDataType = type;
            Name = name;
            Description = description;
            ReadOnlyRecommended = readOnlyRecommended;
        }

        public SettingDataType SettingDataType { get; init; }
        public string Name { get; init; }

        public string Description { get; init; }

        public abstract object GetValue();

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
