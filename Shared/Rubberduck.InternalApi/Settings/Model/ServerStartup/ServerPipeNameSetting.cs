using Rubberduck.InternalApi.Settings.Model;

namespace Rubberduck.InternalApi.Settings.Model.ServerStartup
{
    /// <summary>
    /// The name of the named pipe, when transport type uses pipes.
    /// </summary>
    public record class ServerPipeNameSetting : StringRubberduckSetting
    {
        public ServerPipeNameSetting()
        {
            Tags = SettingTags.ReadOnlyRecommended | SettingTags.Advanced;
            IsRequired = true;
            MinLength = 7;
            MaxLength = 255;
        }
    }
}
