using System.Collections.Generic;
using System.Threading.Tasks;
using Rubberduck.SettingsProvider;

namespace Rubberduck.Settings
{
    public class HotkeyConfigProvider : ConfigurationServiceBase<HotkeySettings>
    {
        private readonly IEnumerable<HotkeySetting> _defaultHotkeys;

        public HotkeyConfigProvider(IAsyncPersistenceService<HotkeySettings> persister)
            : base(persister, null /*new DefaultSettings<HotkeySettings, Properties.Settings>()*/)
        {
            //_defaultHotkeys = new DefaultSettings<HotkeySetting, Properties.Settings>().Defaults;
        }

        public async override Task<HotkeySettings> ReadAsync()
        {
            var prototype = new HotkeySettings(_defaultHotkeys);

            // Loaded settings don't contain defaults, so we need to use the `Settings` property to combine user settings with defaults.
            var loaded = await LoadCacheValueAsync();
            if (loaded != null)
            {
                prototype.Settings = loaded.Settings;
            }

            return prototype;
        }

        public async override Task<HotkeySettings> ReadDefaultsAsync()
        {
            return await Task.FromResult(new HotkeySettings(_defaultHotkeys));
        }
    }
}
