using Rubberduck.Settings;
using System;
using System.Threading.Tasks;

namespace Rubberduck.SettingsProvider
{
    public class ConfigurationServiceBase<T> : IConfigurationService<T>
        where T : class, new()
    {
        private readonly IAsyncPersistenceService<T> _persister;
        protected readonly IDefaultSettings<T> Defaults;

        //private readonly object valueLock = new object();
        protected T CurrentValue;

        public ConfigurationServiceBase(IAsyncPersistenceService<T> persister, IDefaultSettings<T> defaultSettings)
        {
            _persister = persister;
            Defaults = defaultSettings;
        }

        protected void OnSettingsChanged(TrackedSettingValue trackedSettings = TrackedSettingValue.None)
        {
            var eventArgs = new ConfigurationChangedEventArgs(trackedSettings);
            SettingsChanged?.Invoke(this, eventArgs);
        }

        public event EventHandler<ConfigurationChangedEventArgs> SettingsChanged;

        protected async Task<T> LoadCacheValueAsync()
        {
            var fromStorage = await _persister.LoadAsync();
            if (CurrentValue is null)
            {
                T defaults = await ReadDefaultsAsync();
                T newValue = fromStorage ?? defaults;
                CurrentValue = newValue;
            }
            return CurrentValue;
        }

        public virtual async Task<T> ReadAsync()
        {
            return await LoadCacheValueAsync();
        }

        public async virtual Task<T> ReadDefaultsAsync()
        {
            return await Task.FromResult(Defaults?.Default);
        }

        protected async Task PersistValueAsync(T settings)
        {
            // purge current value
            CurrentValue = null;
            await _persister.SaveAsync(settings);
        }

        public async virtual Task SaveAsync(T settings)
        {
            await PersistValueAsync(settings);
            OnSettingsChanged();
        }

        public async virtual Task<T> ImportAsync(string path)
        {
            T loaded = await _persister.LoadAsync(path);
            await SaveAsync(loaded);
            return await ReadAsync();
        }

        public async virtual Task ExportAsync(string path)
        {
            T current = await ReadAsync();
            await _persister.SaveAsync(current, path);
        }
    }
}
