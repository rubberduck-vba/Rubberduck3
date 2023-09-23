using Rubberduck.SettingsProvider;

namespace Rubberduck.Settings
{
    public class AutoCompleteConfigProvider : ConfigurationServiceBase<AutoCompleteSettings>
    {
        public AutoCompleteConfigProvider(IAsyncPersistenceService<AutoCompleteSettings> persister)
            : base(persister, null /*new DefaultSettings<AutoCompleteSettings, Properties.Settings>()*/) { }
    }
}
