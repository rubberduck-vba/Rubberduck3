using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Rubberduck.SettingsProvider;

namespace Rubberduck.Settings
{
    public class ToDoListConfigProvider : ConfigurationServiceBase<ToDoListSettings>
    {
        private readonly IEnumerable<ToDoMarker> _defaultMarkers;
        private readonly ObservableCollection<ToDoGridViewColumnInfo> _toDoExplorerColumns;

        public ToDoListConfigProvider(IAsyncPersistenceService<ToDoListSettings> persister)
            : base(persister, null /*new DefaultSettings<ToDoListSettings, Properties.Settings>()*/)
        {
            //_defaultMarkers = new DefaultSettings<ToDoMarker, Properties.Settings>().Defaults;

            //var gvciDefaults = new DefaultSettings<ToDoGridViewColumnInfo, Properties.Settings>().Defaults;
            //_toDoExplorerColumns = new ObservableCollection<ToDoGridViewColumnInfo>(gvciDefaults);
        }
        
        public async override Task<ToDoListSettings> ReadDefaultsAsync()
        {
            return await Task.FromResult(new ToDoListSettings(_defaultMarkers, _toDoExplorerColumns));
        }

        public async override Task<ToDoListSettings> ReadAsync()
        {
            var toDoListSettings = await base.ReadAsync();

            if (toDoListSettings.ColumnHeadersInformation is null
                || toDoListSettings.ColumnHeadersInformation.Count == 0)
            {
                toDoListSettings.ColumnHeadersInformation = _toDoExplorerColumns;
            }

            return toDoListSettings;
        }
    }
}
