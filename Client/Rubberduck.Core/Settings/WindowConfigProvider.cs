using Rubberduck.SettingsProvider;

namespace Rubberduck.Settings
{
    public class WindowConfigProvider : ConfigurationServiceBase<WindowSettings>
    {
        public WindowConfigProvider(IPersistenceService<WindowSettings> persister)
            : base(persister, null /*new DefaultSettings<WindowSettings, Properties.Settings>()*/) { }
    }
}
