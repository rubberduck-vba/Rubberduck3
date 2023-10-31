using System;

namespace Rubberduck.SettingsProvider.Model.ServerStartup
{
    [TelemetrySensitive]
    /// <summary>
    /// The physical location of the server executable.
    /// </summary>
    public class ServerExecutablePathSetting : UriRubberduckSetting
    {
        public ServerExecutablePathSetting()
        {
            Tags = SettingTags.Advanced | SettingTags.ReadOnlyRecommended;
        }
    }
}
