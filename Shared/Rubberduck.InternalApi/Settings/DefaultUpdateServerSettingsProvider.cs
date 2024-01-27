using Rubberduck.InternalApi.Settings.Model.UpdateServer;

namespace Rubberduck.InternalApi.Settings;

public class DefaultUpdateServerSettingsProvider : IDefaultSettingsProvider<UpdateServerSettings>
{
    public UpdateServerSettings Default { get; } = UpdateServerSettings.Default;
}
