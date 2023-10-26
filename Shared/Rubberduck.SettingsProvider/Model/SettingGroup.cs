﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Rubberduck.SettingsProvider.Model
{
    public abstract record class SettingGroup : RubberduckSetting
    {
        protected SettingGroup(string name, string description)
            : base(SettingDataType.ObjectSetting, name, description)
        {
        }

        protected abstract IEnumerable<RubberduckSetting> Settings { get; init; }
        public Dictionary<string, string> Values => Settings.ToDictionary(
            setting => setting.Name,
            setting => setting.GetValue() is Array values ? $"[\"{string.Join("\",\"", values)}\"]" : setting.GetValue().ToString() ?? string.Empty);

        public sealed override object GetValue() => Settings;
    }
}
