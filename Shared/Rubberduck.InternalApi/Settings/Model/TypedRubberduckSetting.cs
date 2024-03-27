using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.Settings.Model;

public record class TypedRubberduckSetting<TValue> : RubberduckSetting
{
    private TValue _typedValue;
    /// <summary>
    /// The current value/configuration of this setting.
    /// </summary>
    [JsonIgnore]
    public TValue TypedValue => _typedValue;

    /// <summary>
    /// The value returned by <c>Value</c> when a setting was not modified.
    /// </summary>
    [JsonIgnore]
    public TValue DefaultValue { get; init; }

    public override object Value
    {
        get => _typedValue;
        set
        {
            if (SettingDataType == SettingDataType.SettingGroup)
            {
                if (value is JsonElement json && json.ValueKind == JsonValueKind.Array)
                {
                    object values = json.EnumerateArray().Select(item => item.Deserialize<RubberduckSetting>()).ToArray();
                    _typedValue = (TValue)values;
                }
                else
                {
                    _typedValue = (TValue)value; //(TValue)((IEnumerable<RubberduckSetting>)value).ToArray();
                }
            }
            else if (SettingDataType == SettingDataType.EnumSettingGroup)
            {
                // FIXME
                if (typeof(TValue) == typeof(BooleanRubberduckSetting[]) && value is RubberduckSetting[])
                {
                    _typedValue = (TValue)Convert.ChangeType(((RubberduckSetting[])value).Cast<BooleanRubberduckSetting>().ToArray(), typeof(TValue));
                }
                else if (value is not JsonElement)
                {
                    _typedValue = (TValue)value;
                }
                //if (value is JsonElement json && json.ValueKind == JsonValueKind.Array)
                //{

                //    object values = json.EnumerateArray().Select(item => item.Deserialize<BooleanRubberduckSetting>()).ToArray();
                //    _typedValue = (TValue)values;
                //}
                //else
                //{
                //    try
                //    {
                //        _typedValue = (TValue)value;
                //    }
                //    catch (Exception)
                //    {
                //    }
                //}
            }
            else if (SettingDataType != SettingDataType.NotSet)
            {
                if (value is JsonElement json)
                {
                    _typedValue = json.Deserialize<TValue>()!;
                }
                else
                {
                    _typedValue = (TValue)value;
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
