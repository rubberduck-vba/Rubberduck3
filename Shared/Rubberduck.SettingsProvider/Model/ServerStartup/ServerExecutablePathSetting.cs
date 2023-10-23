using System;

namespace Rubberduck.SettingsProvider.Model.ServerStartup
{
    public record class ServerExecutablePathSetting : RubberduckSetting<Uri>
    {
        // TODO localize
        private static readonly string _description = "The physical location of the server executable.";

        public ServerExecutablePathSetting(string name, Uri defaultValue)
            : this(name, defaultValue, defaultValue) { }

        public ServerExecutablePathSetting(string name, Uri defaultValue, Uri value) 
            : base(SettingDataType.UriSetting, name, _description, defaultValue, value, readOnlyRecommended: true)
        {
        }
    }
}
