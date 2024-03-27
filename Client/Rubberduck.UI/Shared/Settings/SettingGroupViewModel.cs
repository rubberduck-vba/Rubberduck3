using Rubberduck.InternalApi.Settings.Model;
using Rubberduck.Resources.v3;
using Rubberduck.UI.Command.Abstract;
using Rubberduck.UI.Services;
using Rubberduck.UI.Services.Settings;
using Rubberduck.UI.Shared.Settings.Abstract;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Rubberduck.UI.Shared.Settings
{
    public class SettingGroupViewModel : ViewModelBase, ISettingGroupViewModel
    {
        private readonly RubberduckSetting _settingGroup;
        private readonly Timer _idleTimer;
        private TimeSpan IdleDelay => UIServiceHelper.Instance!.Settings.EditorSettings.IdleTimerDuration;

        internal SettingGroupViewModel(/* designer */)
        {
        }

        public SettingGroupViewModel(TypedSettingGroup settingGroup, IEnumerable<ISettingViewModel> items)
        {
            // readonly-recommended padlock makes weird UX on setting groups
            _settingGroup = settingGroup with { Tags = settingGroup.Tags & ~SettingTags.ReadOnlyRecommended };
            _idleTimer = new Timer(OnIdleTimerTick, null, IdleDelay, Timeout.InfiniteTimeSpan);
            _selection = items.First();

            foreach (var item in items)
            {
                item.ShowSettingGroup = settingGroup.Key == SettingsWindowViewModel.SearchResultsSettingGroupName;
            }

            Items = new ObservableCollection<ISettingViewModel>(items);

            ItemsView = CollectionViewSource.GetDefaultView(Items);
            ItemsView.SortDescriptions.Add(new SortDescription(nameof(ISettingViewModel.IsSettingGroup), ListSortDirection.Ascending));
            ItemsView.SortDescriptions.Add(new SortDescription(nameof(ISettingViewModel.Name), ListSortDirection.Ascending));
            ItemsView.Filter = value => string.IsNullOrWhiteSpace(_searchString) || ((ISettingViewModel)value).IsSearchResult(_searchString);

            IsEnabled = true;
            EnableAllItemsCommand = new DelegateCommand(UIServiceHelper.Instance!,
                parameter =>
                {
                    var isEnabled = (bool)parameter!;
                    foreach (var item in Items.OfType<BooleanSettingViewModel>())
                    {
                        item.Value = isEnabled;
                    }
                });
        }

        public SettingGroupViewModel(TypedRubberduckSetting<BooleanRubberduckSetting[]> settingGroup, IEnumerable<ISettingViewModel> items)
        {
            // readonly-recommended padlock makes weird UX on setting groups
            _settingGroup = settingGroup with { Tags = settingGroup.Tags & ~SettingTags.ReadOnlyRecommended };
            _idleTimer = new Timer(OnIdleTimerTick, null, IdleDelay, Timeout.InfiniteTimeSpan);
            _selection = items.First();

            Items = new ObservableCollection<ISettingViewModel>(items);

            ItemsView = CollectionViewSource.GetDefaultView(Items);
            ItemsView.SortDescriptions.Add(new SortDescription(nameof(ISettingViewModel.IsSettingGroup), ListSortDirection.Ascending));
            ItemsView.SortDescriptions.Add(new SortDescription(nameof(ISettingViewModel.Name), ListSortDirection.Ascending));
            ItemsView.Filter = value => string.IsNullOrWhiteSpace(_searchString) || ((ISettingViewModel)value).IsSearchResult(_searchString);

            IsEnabled = true;
            EnableAllItemsCommand = new DelegateCommand(UIServiceHelper.Instance!,
                parameter =>
                {
                    var isEnabled = Items.OfType<BooleanSettingViewModel>().Any(e => !e.Value);
                    foreach (var item in Items.OfType<BooleanSettingViewModel>())
                    {
                        item.Value = isEnabled;
                    }
                });
        }

        public SettingGroupViewModel(TypedRubberduckSetting<RubberduckSetting[]> settingGroup, IEnumerable<ISettingViewModel> items)
        {
            // readonly-recommended padlock makes weird UX on setting groups
            _settingGroup = settingGroup with { Tags = settingGroup.Tags & ~SettingTags.ReadOnlyRecommended };
            _idleTimer = new Timer(OnIdleTimerTick, null, IdleDelay, Timeout.InfiniteTimeSpan);
            _selection = items.First();

            Items = new ObservableCollection<ISettingViewModel>(items);

            ItemsView = CollectionViewSource.GetDefaultView(Items);
            ItemsView.SortDescriptions.Add(new SortDescription(nameof(ISettingViewModel.IsSettingGroup), ListSortDirection.Ascending));
            ItemsView.SortDescriptions.Add(new SortDescription(nameof(ISettingViewModel.Name), ListSortDirection.Ascending));
            ItemsView.Filter = value => string.IsNullOrWhiteSpace(_searchString) || ((ISettingViewModel)value).IsSearchResult(_searchString);

            IsEnabled = true;
            EnableAllItemsCommand = new DelegateCommand(UIServiceHelper.Instance!,
                parameter =>
                {
                    var isEnabled = (bool)parameter!;
                    foreach (var item in Items.OfType<BooleanSettingViewModel>())
                    {
                        item.Value = isEnabled;
                    }
                });
        }

        public ICollectionView ItemsView { get; init; }
        public ObservableCollection<ISettingViewModel> Items { get; init; }

        private void OnIdleTimerTick(object? state) => Application.Current.Dispatcher.Invoke(() => ItemsView?.Refresh());
        private void ResetIdleTimer(bool immediate = false) => _idleTimer.Change(immediate ? TimeSpan.Zero : IdleDelay, Timeout.InfiniteTimeSpan);

        public bool ShowSettingGroup { get; set; }
        public bool IsSettingGroup => true;
        public bool IsSearchResult(string search) => 
            Name.Contains(search, StringComparison.InvariantCultureIgnoreCase)
            || Description.Contains(search, StringComparison.InvariantCultureIgnoreCase);

        private string? _searchString;
        public string? SearchString 
        {
            get => _searchString;
            set
            {
                if (_searchString != value)
                {
                    _searchString = value;
                    OnPropertyChanged();
                }
                ResetIdleTimer(immediate: value is null);
            }
        }

        private ISettingViewModel _selection;
        public ISettingViewModel Selection 
        {
            get => _selection;
            set
            {
                if (_selection != value)
                {
                    _selection = value;
                    OnPropertyChanged();
                }
            }
        }

        public SettingDataType SettingDataType => _settingGroup.SettingDataType;
        public string Key => _settingGroup.Key;
        
        public string SettingGroupKey { get; set; }
        public string Name => SettingsUI.ResourceManager.GetString($"{_settingGroup.Key}_Title") ?? $"[missing key:{_settingGroup.Key}_Title]";

        public string Description => SettingsUI.ResourceManager.GetString($"{_settingGroup.Key}_Description") ?? $"[missing key:{_settingGroup.Key}_Description]";
        public bool AllowToggleAllBooleans => Items.All(e => e is BooleanSettingViewModel);

        public ICommand EnableAllItemsCommand { get; }

        public SettingTags Tags => _settingGroup.Tags;
        public bool IsReadOnlyRecommended => !IsExpanded && Tags.HasFlag(SettingTags.ReadOnlyRecommended);
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

        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    OnPropertyChanged();
                }
            }
        }

        public RubberduckSetting ToSetting() => _settingGroup with { Value = Items.Select(e => e.ToSetting()).ToArray() };
    }
}
