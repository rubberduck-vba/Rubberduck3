namespace Rubberduck.SettingsProvider.Model.ServerStartup
{
    public record class ServerPipeNameSetting : RubberduckSetting<string>
    {
        // TODO localize
        private static readonly string _description = "The name of the named pipe, when transport type uses pipes.";
        public ServerPipeNameSetting(string name, string defaultValue)
            : this(name, defaultValue, defaultValue) { }

        public ServerPipeNameSetting(string name, string defaultValue, string value)
            : base(SettingDataType.TextSetting, name, _description, defaultValue, value, readOnlyRecommended: true)
        {
        }
    }
}
