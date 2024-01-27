﻿using System.IO;
using System;
using Rubberduck.InternalApi.Settings.Model;

namespace Rubberduck.InternalApi.Settings.Model.General;

public record class TemplatesLocationSetting : UriRubberduckSetting
{
    private static readonly string LocalAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

    public static Uri DefaultSettingValue { get; } = new(Path.Combine(LocalAppData, "Rubberduck", "Templates"));

    public TemplatesLocationSetting()
    {
        DefaultValue = DefaultSettingValue;
        Tags = SettingTags.Advanced | SettingTags.ReadOnlyRecommended;
    }
}
