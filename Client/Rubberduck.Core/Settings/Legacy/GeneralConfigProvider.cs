using System;
using System.Threading.Tasks;
using Rubberduck.SettingsProvider;
using Properties = Rubberduck.Core.Properties;

namespace Rubberduck.Settings
{
    public class GeneralConfigProvider : ConfigurationServiceBase<GeneralSettings>
    {

        public GeneralConfigProvider(IAsyncPersistenceService<GeneralSettings> persister)
            : base(persister, new DefaultSettings<GeneralSettings, Properties.Settings>())
        {
        }

        public async override Task<GeneralSettings> ReadAsync()
        {
            var before = CurrentValue;
            var updated = await LoadCacheValueAsync();
            CheckForEventsToRaise(before, updated);
            return updated;
        }

        public async override Task SaveAsync(GeneralSettings settings)
        {
            var before = CurrentValue;
            await PersistValueAsync(settings);
            CheckForEventsToRaise(before, settings);
            OnSettingsChanged();
        }

        private void CheckForEventsToRaise(GeneralSettings before, GeneralSettings after)
        {
            if (before is null || !Equals(after.Language, before.Language))
            {
                OnLanguageChanged(EventArgs.Empty);
            }
            if (before is null ||
                after.IsAutoSaveEnabled != before.IsAutoSaveEnabled ||
                after.AutoSavePeriod != before.AutoSavePeriod)
            {
                OnAutoSaveSettingsChanged(EventArgs.Empty);
            }
        }

        public event EventHandler LanguageChanged;
        protected virtual void OnLanguageChanged(EventArgs e)
        {
            LanguageChanged?.Invoke(this, e);
        }

        public event EventHandler AutoSaveSettingsChanged;
        protected virtual void OnAutoSaveSettingsChanged(EventArgs e)
        {
            AutoSaveSettingsChanged?.Invoke(this, e);
        }
    }
}