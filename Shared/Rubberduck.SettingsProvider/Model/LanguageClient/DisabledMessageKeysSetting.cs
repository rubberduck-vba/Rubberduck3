using System;

namespace Rubberduck.SettingsProvider.Model.LanguageClient
{
    /// <summary>
    /// Identifies UI messages that should not be displayed.
    /// </summary>
    public record class DisabledMessageKeysSetting : TypedRubberduckSetting<string[]>
    {
        public static string[] DefaultSettingValue { get; } = Array.Empty<string>();

        public DisabledMessageKeysSetting()
        {
            SettingDataType = SettingDataType.ListSetting;
            DefaultValue = DefaultSettingValue;
        }
    }
}
