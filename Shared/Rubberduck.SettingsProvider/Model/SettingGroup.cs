using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rubberduck.SettingsProvider.Model
{
    public abstract record class SettingGroup : RubberduckSetting
    {
        protected SettingGroup(string name, string description)
            : base(SettingDataType.ObjectSetting, name, description)
        {
        }

        private IEnumerable<RubberduckSetting> _settings;
        protected IEnumerable<RubberduckSetting> Settings
        {
            get => _settings;
            init
            {
                string GetStringValue(object obj)
                {
                    if (obj is Array values)
                    {
                        var jsonValues = new List<string>();
                        foreach (var value in values)
                        {
                            if (value is string)
                            {
                                jsonValues.Add(value.ToString() ?? string.Empty);
                            }
                            if (value is object)
                            {
                                jsonValues.Add(JsonSerializer.Serialize(value));
                            }
                        }
                        return $"[\"{string.Join("\",\"", jsonValues)}\"]";
                    }
                    else
                    {
                        return obj.ToString() ?? string.Empty;
                    }
                };

                if (value is not null)
                {
                    _settings = value;
                    Values = Settings.ToDictionary(
                        setting => setting.Name, 
                        setting => GetStringValue(setting.GetValue())
                    );
                }
                else
                {
                    // WTF?
                }
            }
        }

        [JsonIgnore]
        public Dictionary<string, string> Values { get; init; }

        public sealed override object GetValue() => Settings;
    }
}
