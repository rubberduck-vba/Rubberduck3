using System;
using System.Collections.Generic;
using System.Linq;

namespace Rubberduck.InternalApi.Settings.Model.LanguageClient;

/// <summary>
/// Identifies UI messages that should not be displayed.
/// </summary>
public record class DisabledMessageKeysSetting : TypedRubberduckSetting<string[]>
{
    public static string[] DefaultSettingValue { get; } = Array.Empty<string>();

    public static void DisableMessageKey(string key, RubberduckSettingsProvider provider)
    {
        var generalSettings = provider.Settings.GeneralSettings;
        var newValue = generalSettings.DisabledMessageKeys.Append(key);
        var newSetting = generalSettings.TypedValue.OfType<DisabledMessageKeysSetting>().Single().WithValue(newValue);
        var newGeneralSettings = generalSettings.WithSetting(newSetting);
        var newRubberduckSettings = (RubberduckSettings)provider.Settings.WithSetting(newGeneralSettings);
        provider.Write(newRubberduckSettings);
    }

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
