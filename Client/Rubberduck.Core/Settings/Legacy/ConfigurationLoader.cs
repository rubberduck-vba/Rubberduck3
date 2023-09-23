using System;
using System.Threading.Tasks;
using Rubberduck.SettingsProvider;
//using Rubberduck.SmartIndenter;
//using Rubberduck.UnitTesting.Settings;
//using Rubberduck.CodeAnalysis.Settings;

namespace Rubberduck.Settings
{
    public class ConfigurationLoader : IConfigurationService<Configuration>
    {
        private readonly IConfigurationService<GeneralSettings> _generalProvider;
        //private readonly IConfigurationService<HotkeySettings> _hotkeyProvider;
        //private readonly IConfigurationService<AutoCompleteSettings> _autoCompleteProvider;
        //private readonly IConfigurationService<ToDoListSettings> _todoProvider;
        //private readonly IConfigurationService<CodeInspectionSettings> _inspectionProvider;
        //private readonly IConfigurationService<UnitTestSettings> _unitTestProvider;
        //private readonly IConfigurationService<IndenterSettings> _indenterProvider;
        //private readonly IConfigurationService<WindowSettings> _windowProvider;

        public ConfigurationLoader(IConfigurationService<GeneralSettings> generalProvider
            //IConfigurationService<HotkeySettings> hotkeyProvider, 
            //IConfigurationService<AutoCompleteSettings> autoCompleteProvider, 
            //IConfigurationService<ToDoListSettings> todoProvider,
            //IConfigurationService<CodeInspectionSettings> inspectionProvider, 
            //IConfigurationService<UnitTestSettings> unitTestProvider, 
            //IConfigurationService<IndenterSettings> indenterProvider, 
            //IConfigurationService<WindowSettings> windowProvider
            )
        {
            _generalProvider = generalProvider;
            //_hotkeyProvider = hotkeyProvider;
            //_autoCompleteProvider = autoCompleteProvider;
            //_todoProvider = todoProvider;
            //_inspectionProvider = inspectionProvider;
            //_unitTestProvider = unitTestProvider;
            //_indenterProvider = indenterProvider;
            //_windowProvider = windowProvider;
        }

        /// <summary>
        /// Loads the configuration from Rubberduck.config xml file.
        /// </summary>
        // marked virtual for Mocking
        public async virtual Task<Configuration> ReadAsync()
        {
            var readerTasks = new[]
            {
                _generalProvider.ReadAsync(),
                //...
            };
            await Task.WhenAll(readerTasks);

            var config = new Configuration
            {
                UserSettings = new UserSettings
                (
                    readerTasks[0].Result,
                    null, //_hotkeyProvider.Read(),
                    null, //_autoCompleteProvider.Read(),
                    null, //_todoProvider.Read(),
                    null  //_windowProvider.Read()
                )
            };
            return config;
        }

        public async Task<Configuration> ReadDefaultsAsync()
        {
            var readerTasks = new[]
            {
                _generalProvider.ReadDefaultsAsync(),
                //...
            };
            await Task.WhenAll(readerTasks);

            return new Configuration
            {
                UserSettings = new UserSettings
                (
                    readerTasks[0].Result,
                    null, //_hotkeyProvider.ReadDefaults(),
                    null, //_autoCompleteProvider.ReadDefaults(),
                    null, //_todoProvider.ReadDefaults(),
                    null  //_windowProvider.ReadDefaults()
                )
            };
        }
        
        public async Task SaveAsync(Configuration toSerialize)
        {
            var generalSettings = await _generalProvider.ReadAsync();
            var langChanged = generalSettings.Language.Code != toSerialize.UserSettings.GeneralSettings.Language.Code;
            //var oldInspectionSettings = _inspectionProvider.Read().CodeInspections.Select(s => Tuple.Create(s.Name, s.Severity));
            //var newInspectionSettings = toSerialize.UserSettings.CodeInspectionSettings.CodeInspections.Select(s => Tuple.Create(s.Name, s.Severity));
            //var inspectionsChanged = !oldInspectionSettings.SequenceEqual(newInspectionSettings);
            //var inspectOnReparse = toSerialize.UserSettings.CodeInspectionSettings.RunInspectionsOnSuccessfulParse;

            //var oldAutoCompleteSettings = _autoCompleteProvider.Read();
            //var newAutoCompleteSettings = toSerialize.UserSettings.AutoCompleteSettings;
            //var autoCompletesChanged = oldAutoCompleteSettings?.Equals(newAutoCompleteSettings) ?? false;

            await _generalProvider.SaveAsync(toSerialize.UserSettings.GeneralSettings);
            //_hotkeyProvider.Save(toSerialize.UserSettings.HotkeySettings);
            //_autoCompleteProvider.Save(toSerialize.UserSettings.AutoCompleteSettings);
            //_todoProvider.Save(toSerialize.UserSettings.ToDoListSettings);
            //_inspectionProvider.Save(toSerialize.UserSettings.CodeInspectionSettings);
            //_unitTestProvider.Save(toSerialize.UserSettings.UnitTestSettings);
            //_indenterProvider.Save(toSerialize.UserSettings.IndenterSettings);
            //_windowProvider.Save(toSerialize.UserSettings.WindowSettings);

            OnSettingsChanged(new ConfigurationChangedEventArgs(false /*inspectOnReparse*/, langChanged, false /*inspectionsChanged*/, false /*autoCompletesChanged*/));
        }

        public event EventHandler<ConfigurationChangedEventArgs> SettingsChanged;
        protected virtual void OnSettingsChanged(ConfigurationChangedEventArgs e)
        {
            SettingsChanged?.Invoke(this, e);
        }

        public async Task<Configuration> ImportAsync(string fileName)
        {
            var importTasks = new[]
            {
                _generalProvider.ImportAsync(fileName),
                //...
            };
            await Task.WhenAll(importTasks);

            return new Configuration
            {
                UserSettings = new UserSettings
                (
                    importTasks[0].Result,
                    null, //_hotkeyProvider.Import(fileName),
                    null, //_autoCompleteProvider.Import(fileName),
                    null, //_todoProvider.Import(fileName),
                    null  //_windowProvider.Import(fileName)
                )
            };
        }

        public async Task ExportAsync(string fileName)
        {
            var exportTasks = new[]
            {
                _generalProvider.ExportAsync(fileName),
                //...
            };
            await Task.WhenAll(exportTasks);
            //_hotkeyProvider.Export(fileName);
            //_autoCompleteProvider.Export(fileName);
            //_todoProvider.Export(fileName);
            //_inspectionProvider.Export(fileName);
            //_unitTestProvider.Export(fileName);
            //_indenterProvider.Export(fileName);
            //_windowProvider.Export(fileName);
        }
    }
}
