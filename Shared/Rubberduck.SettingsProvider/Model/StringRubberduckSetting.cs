using System;
using System.Collections.Generic;
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
}
