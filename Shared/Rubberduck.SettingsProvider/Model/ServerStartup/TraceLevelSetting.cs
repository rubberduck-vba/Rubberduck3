using Rubberduck.InternalApi.Settings;

namespace Rubberduck.SettingsProvider.Model.ServerStartup
{
    public record class TraceLevelSetting : RubberduckSetting<MessageTraceLevel>
    {
        // TODO localize
        private static readonly string _description = "The server trace verbosity level.";

        public TraceLevelSetting(string name, MessageTraceLevel defaultValue)
            : this(name, defaultValue, defaultValue) { }

        public TraceLevelSetting(string name, MessageTraceLevel defaultValue, MessageTraceLevel value)
            : base(name, value, SettingDataType.EnumSetting, defaultValue) { }
    }
}
