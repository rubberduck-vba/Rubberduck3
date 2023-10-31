﻿using Microsoft.Extensions.Logging;

namespace Rubberduck.SettingsProvider.Model.General
{
    /// <summary>
    /// The minimum log level for log messages to be written to log files.
    /// </summary>
    public class LogLevelSetting : TypedRubberduckSetting<LogLevel>
    {
        public static LogLevel DefaultSettingValue { get; } = LogLevel.Trace;

        public LogLevelSetting()
        {
            SettingDataType = SettingDataType.EnumValueSetting;
            DefaultValue = DefaultSettingValue;
        }
    }
}
