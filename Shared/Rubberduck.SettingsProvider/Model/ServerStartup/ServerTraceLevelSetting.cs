using Rubberduck.InternalApi.Settings;

namespace Rubberduck.SettingsProvider.Model.ServerStartup
{
    public record class ServerTraceLevelSetting : RubberduckSetting<ServerTraceLevel>
    {
        // TODO localize
        private static readonly string _description = "The server trace verbosity level.";

        public ServerTraceLevelSetting(string name, ServerTraceLevel defaultValue)
            : this(name, defaultValue, defaultValue) { }

        public ServerTraceLevelSetting(string name, ServerTraceLevel defaultValue, ServerTraceLevel value)
            : base(SettingDataType.EnumSetting, name, _description, defaultValue, value)
        {
        }
    }
}
