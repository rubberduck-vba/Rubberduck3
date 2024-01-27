﻿using System;

namespace Rubberduck.InternalApi.Settings.Model.LanguageClient;

public record class ExitNotificationDelaySetting : TypedRubberduckSetting<TimeSpan>
{
    public static TimeSpan DefaultSettingValue { get; } = TimeSpan.FromSeconds(1);

    public ExitNotificationDelaySetting()
    {
        SettingDataType = SettingDataType.TimeSpanSetting;
        DefaultValue = DefaultSettingValue;
        Tags = SettingTags.ReadOnlyRecommended;
    }
}
