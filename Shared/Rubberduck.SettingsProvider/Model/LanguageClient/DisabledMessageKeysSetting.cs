using System;
using System.Collections.Generic;
using System.Linq;

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

        public DisabledMessageKeysSetting WithDisabledMessageKeys(params string[] keys)
        {
            return this with { Value = new HashSet<string>(TypedValue.Concat(keys)).ToArray() };
        }

        public DisabledMessageKeysSetting WithoutDisabledMessageKeys(params string[] keys)
        {
            return this with { Value = TypedValue.Except(keys).ToArray() };
        }
    }
}
