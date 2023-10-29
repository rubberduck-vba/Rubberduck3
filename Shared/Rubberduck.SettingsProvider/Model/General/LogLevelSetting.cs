using Microsoft.Extensions.Logging;

namespace Rubberduck.SettingsProvider.Model
{
    public record class LogLevelSetting : RubberduckSetting<LogLevel>
    {
        public static LogLevel DefaultSettingValue { get; } = LogLevel.Trace;

        private static readonly string _description = "The minimum log level for log messages to be written to log files.";


        public LogLevelSetting() : this(DefaultSettingValue) { }

        public LogLevelSetting(LogLevel value)
            : base(nameof(LogLevelSetting), value, SettingDataType.EnumSetting, DefaultSettingValue) { }
    }
}
