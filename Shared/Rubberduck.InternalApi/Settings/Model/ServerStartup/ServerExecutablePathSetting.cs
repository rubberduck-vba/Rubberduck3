namespace Rubberduck.InternalApi.Settings.Model.ServerStartup;

[TelemetrySensitive]
/// <summary>
/// The physical location of the server executable.
/// </summary>
public record class ServerExecutablePathSetting : UriRubberduckSetting
{
    public ServerExecutablePathSetting()
    {
        Tags = SettingTags.Advanced | SettingTags.ReadOnlyRecommended;
    }
}
