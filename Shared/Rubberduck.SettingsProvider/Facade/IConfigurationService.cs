using System;
using System.Threading.Tasks;

namespace Rubberduck.SettingsProvider
{
    public interface IConfigurationService<T>
    {
        Task<T> ReadAsync();
        Task<T> ReadDefaultsAsync();

        Task SaveAsync(T settings);
        Task<T> ImportAsync(string fileName);
        Task ExportAsync(string fileName);

        event EventHandler<ConfigurationChangedEventArgs> SettingsChanged;
    }

    [Flags]
    public enum TrackedSettingValue
    {
        None,
        DisplayLanguage = 1,
        RunInspectionOnSuccessfulParse = 2,
        InspectionSettings = 4,
        CompletionSettings = 8,
    }

    public class ConfigurationChangedEventArgs : EventArgs
    {
        public ConfigurationChangedEventArgs(TrackedSettingValue changedSettings = TrackedSettingValue.None)
        {
            ChangedSettings = changedSettings;
        }

        public ConfigurationChangedEventArgs(bool runInspections, bool languageChanged, bool inspectionSettingsChanged, bool autoCompleteSettingsChanged)
        {
            var changedSettings = TrackedSettingValue.None;

            if (runInspections)
            {
                changedSettings |= TrackedSettingValue.RunInspectionOnSuccessfulParse;
            }

            if (languageChanged)
            {
                changedSettings |= TrackedSettingValue.DisplayLanguage;
            }

            if (inspectionSettingsChanged)
            {
                changedSettings |= TrackedSettingValue.InspectionSettings;
            }

            if (autoCompleteSettingsChanged)
            {
                changedSettings |= TrackedSettingValue.CompletionSettings;
            }

            ChangedSettings = changedSettings;
        }

        public TrackedSettingValue ChangedSettings { get; }
        public bool DisplayLanguageChanged => ChangedSettings.HasFlag(TrackedSettingValue.DisplayLanguage);
        public bool RunInspectionsOnReparseChanged => ChangedSettings.HasFlag(TrackedSettingValue.RunInspectionOnSuccessfulParse);
        public bool InspectionSettingsChanged => ChangedSettings.HasFlag(TrackedSettingValue.InspectionSettings);
        public bool AutoCompleteSettingsChanged => ChangedSettings.HasFlag(TrackedSettingValue.CompletionSettings);
    }
}