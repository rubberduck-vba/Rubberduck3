using System;

namespace Rubberduck.SettingsProvider.Model.LanguageClient
{
    public record class DisabledMessageKeysSetting : TypedRubberduckSetting<string[]>
    {
        // TODO localize
        private static readonly string _description = "Identifies UI messages that should not be displayed.";
        public static string[] DefaultSettingValue { get; } = Array.Empty<string>();

        public DisabledMessageKeysSetting()
            : base(nameof(DisabledMessageKeysSetting), DefaultSettingValue, SettingDataType.ListSetting, DefaultSettingValue) { }

        public DisabledMessageKeysSetting(string[] value)
            : base(nameof(DisabledMessageKeysSetting), value, SettingDataType.ListSetting, DefaultSettingValue) { }
    }
}
