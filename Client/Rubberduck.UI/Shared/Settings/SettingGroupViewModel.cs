using Rubberduck.SettingsProvider.Model;
using Rubberduck.UI.Shared.Settings.Abstract;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Rubberduck.UI.Shared.Settings
{
    public class SettingGroupViewModel : ViewModelBase, ISettingGroupViewModel
    {
        private readonly RubberduckSetting _settingGroup;

        internal SettingGroupViewModel()
        {
            /* designer */
        }

        public SettingGroupViewModel(TypedSettingGroup settingGroup, IEnumerable<ISettingViewModel> items)
        {
            _settingGroup = settingGroup;
            Items = new ObservableCollection<ISettingViewModel>(items);
            IsEnabled = !_settingGroup.Tags.HasFlag(SettingTags.ReadOnlyRecommended);
        }

        public SettingGroupViewModel(TypedRubberduckSetting<BooleanRubberduckSetting[]> settingGroup, IEnumerable<ISettingViewModel> items)
        {
            _settingGroup = settingGroup;
            Items = new ObservableCollection<ISettingViewModel>(items);
            IsEnabled = !_settingGroup.Tags.HasFlag(SettingTags.ReadOnlyRecommended);
        }

        public ObservableCollection<ISettingViewModel> Items { get; init; }

        public SettingDataType SettingDataType => _settingGroup.SettingDataType;
        public string Key => _settingGroup.Key;
        public string Name => _settingGroup.Key; // TODO fetch from resources

        public string Description => _settingGroup.Key; // TODO fetch from resources

        public SettingTags Tags => _settingGroup.Tags;
        public bool IsReadOnlyRecommended => Tags.HasFlag(SettingTags.ReadOnlyRecommended);
        public bool IsAdvancedSetting => Tags.HasFlag(SettingTags.Advanced);
        public bool IsExperimental => Tags.HasFlag(SettingTags.Experimental);

        private bool _isEnabled;
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        public RubberduckSetting ToSetting() => _settingGroup with { Value = Items.Select(e => e.ToSetting()).ToArray() };
    }
}
