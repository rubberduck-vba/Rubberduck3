﻿using Rubberduck.InternalApi.Settings.Model;

namespace Rubberduck.InternalApi.Settings.Model.Editor.Tools
{
    public record class DefaultToolWindowLocationSetting : TypedRubberduckSetting<DockingLocation>
    {
        public static DockingLocation DefaultSettingValue { get; } = DockingLocation.None;

        public DefaultToolWindowLocationSetting()
        {
            SettingDataType = SettingDataType.EnumValueSetting;
            DefaultValue = DefaultSettingValue;
        }
    }
}
