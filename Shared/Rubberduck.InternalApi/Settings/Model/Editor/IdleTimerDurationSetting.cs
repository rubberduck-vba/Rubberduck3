using System;

namespace Rubberduck.InternalApi.Settings.Model.Editor;

public record class IdleTimerDurationSetting : TypedRubberduckSetting<TimeSpan>
{
    public static TimeSpan DefaultSettingValue { get; } = TimeSpan.FromMilliseconds(1200);

    public IdleTimerDurationSetting()
    {
        SettingDataType = SettingDataType.TimeSpanSetting;
        DefaultValue = DefaultSettingValue;
        Tags = SettingTags.ReadOnlyRecommended;
    }
}