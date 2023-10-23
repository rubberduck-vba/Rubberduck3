using System;
using System.Text.Json;

namespace Rubberduck.SettingsProvider.Model.LanguageClient
{
    public record class DisabledMessageKeysSetting : RubberduckSetting<string[]>
    {
        public static string[] DefaultSettingValue { get; } = Array.Empty<string>();

        // TODO localize
        private static readonly string _description = "Identifies UI messages that should not be displayed.";

        public DisabledMessageKeysSetting()
            : base(SettingDataType.ListSetting, nameof(DisabledMessageKeysSetting), _description, DefaultSettingValue)
        {
        }

        public DisabledMessageKeysSetting(string[] value)
            : base(SettingDataType.ListSetting, nameof(DisabledMessageKeysSetting), _description, DefaultSettingValue, value)
        {
        }

        public override string ToString() => JsonSerializer.Serialize(Value);
    }
}
