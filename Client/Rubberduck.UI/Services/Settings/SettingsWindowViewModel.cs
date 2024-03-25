using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Settings.Model;
using Rubberduck.Resources;
using Rubberduck.Resources.v3;
using Rubberduck.UI.Chrome;
using Rubberduck.UI.Command;
using Rubberduck.UI.Command.Abstract;
using Rubberduck.UI.Command.SharedHandlers;
using Rubberduck.UI.Shared.Message;
using Rubberduck.UI.Shared.Settings;
using Rubberduck.UI.Shared.Settings.Abstract;
using Rubberduck.UI.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Rubberduck.UI.Services.Settings
{
    public class SettingsWindowViewModel : DialogWindowViewModel, ISettingsWindowViewModel
    {
        public static readonly string SearchResultsSettingGroupName = "SearchResults";

        private readonly IMessageService _message;
        private readonly ISettingViewModelFactory _factory;
        private readonly UIServiceHelper _service;

        private readonly Stack<ISettingGroupViewModel> _backwardNavStack = [];
        private readonly Stack<ISettingGroupViewModel> _forwardNavStack = [];

        public SettingsWindowViewModel(UIServiceHelper service, MessageActionCommand[] actions, 
            IWindowChromeViewModel chrome, IMessageService message, ISettingViewModelFactory factory)
            : base(service, RubberduckUI.Settings, actions, chrome)
        {
            _message = message;
            _factory = factory;
            _service = service;

            service.RunOnMainThread(() => Settings = factory.CreateViewModel(service.Settings));
            SettingGroups = Settings.Items.OfType<ISettingGroupViewModel>().ToList();
            Selection = SettingGroups.FirstOrDefault()!;

            ExpandSettingGroupCommand = new DelegateCommand(service, parameter =>
            {
                if (parameter is ISettingGroupViewModel model)
                {
                    ExecuteExpandSettingGroupCommand(model);
                }
            })
            { 
                Name = nameof(ExpandSettingGroupCommand) 
            };

            ResetSettingsCommand = new DelegateCommand(service, parameter =>
            {
                var model = MessageRequestModel.For(LogLevel.Warning, "ConfirmResetSettings", null!, [MessageAction.AcceptConfirmAction, MessageAction.CancelAction]);
                var result = _message.ShowMessageRequest(model);
                if (result?.MessageAction.IsDefaultAction ?? false)
                {
                    _service.SettingsProvider.Write(RubberduckSettings.Default);

                }
            })
            { 
                Name = nameof(ResetSettingsCommand) 
            };

            ClearSearchTextCommand = new DelegateCommand(service, parameter =>
            {
                if (Selection != null)
                {
                    Selection.SearchString = null!;
                }
            })
            {
                Name = nameof(ClearSearchTextCommand)
            };

            var searchCommand = new DelegateCommand(service, parameter =>
            {
                if (parameter is string text)
                {
                    if (_flattenedSettings is null)
                    {
                        _flattenedSettings = Flatten(_settings!).ToList();
                    }
                    var results = _flattenedSettings;
                    var filteredResults = results.Where(e => e.IsSearchResult(text)).ToList();
                    var vm = new SettingGroupViewModel(service.Settings.WithKey(SearchResultsSettingGroupName), filteredResults);
                    vm.SearchString = text;
                    Selection = vm;
                }
            }, parameter => (parameter is string text) && !string.IsNullOrWhiteSpace(text))
            {
                Name = nameof(NavigationCommands.Search)
            };

            CommandBindings =
            [
                new CommandBinding(DialogCommands.BrowseLocationCommand, DialogCommandHandlers.BrowseLocationCommandBinding_Executed, DialogCommandHandlers.BrowseLocationCommandBinding_CanExecute),
                new CommandBinding(NavigationCommands.BrowseBack, ExecuteNavigateBackward, CanExecuteNavigateBackward),
                new CommandBinding(NavigationCommands.BrowseForward, ExecuteNavigateForward, CanExecuteNavigateForward),
                new CommandBinding(NavigationCommands.Search, searchCommand.ExecutedRouted(), searchCommand.CanExecuteRouted())
            ];
        }

        private IEnumerable<ISettingViewModel>? _flattenedSettings;
        private static IEnumerable<ISettingViewModel> Flatten(ISettingGroupViewModel group)
        {
            group.SettingGroupKey = group.Key;
            var results = new HashSet<ISettingViewModel>();
            foreach (var item in group.Items.Where(e => e is not ISettingGroupViewModel))
            {
                item.SettingGroupKey = group.Key;
                results.Add(item);
            }
            foreach (var subgroup in group.Items.OfType<ISettingGroupViewModel>())
            {
                subgroup.SettingGroupKey = group.Key;
                results.Add(subgroup);

                var flattenedSubgroup = Flatten(subgroup); // recursive
                foreach (var item in flattenedSubgroup)
                {
                    item.SettingGroupKey = subgroup.Key;
                    results.Add(item);
                }
            }
            return results;
        }

        public IEnumerable<ISettingViewModel> FlattenedSettings => _flattenedSettings ??= Flatten(_settings!).ToList();

        public override IEnumerable<CommandBinding> CommandBindings { get; }

        public bool ShowPinButton => false;
        public bool IsPinned { get; set; }

        public ICommand ResetSettingsCommand { get; }
        public ICommand ExpandSettingGroupCommand { get; }
        public ICommand ClearSearchTextCommand { get; }

        private void CanExecuteNavigateBackward(object sender, CanExecuteRoutedEventArgs parameter) => parameter.CanExecute = _backwardNavStack.Count > 0;
        private void ExecuteNavigateBackward(object sender, ExecutedRoutedEventArgs parameter)
        {
            if (!_backwardNavStack.TryPeek(out _))
            {
                return;
            }

            _isManualSelection = false;
            if (_forwardNavStack.Count == 0 || _forwardNavStack.TryPeek(out var next) && next.Key != Selection.Key)
            {
                _forwardNavStack.Push(Selection);
            }

            NextNavKey = Localized(Selection.Key);
            var selection = _backwardNavStack.Pop();
            Selection = selection;

            if (_backwardNavStack.TryPeek(out var previous))
            {
                PreviousNavKey = Localized(previous.Key);
            }
            else
            {
                PreviousNavKey = null!;
            }

            _isManualSelection = true;
        }

        private void CanExecuteNavigateForward(object sender, CanExecuteRoutedEventArgs parameter) => parameter.CanExecute = _forwardNavStack.Count > 0;
        private void ExecuteNavigateForward(object sender, ExecutedRoutedEventArgs parameter)
        {
            if (!_forwardNavStack.TryPeek(out _))
            {
                return;
            }

            _isManualSelection = false;
            if (_backwardNavStack.Count == 0 || _backwardNavStack.TryPeek(out var previous) && previous.Key != Selection.Key)
            {
                _backwardNavStack.Push(Selection);
            }

            PreviousNavKey = Localized(Selection.Key);
            var selection = _forwardNavStack.Pop();
            Selection = selection;

            if (_forwardNavStack.TryPeek(out var next))
            {
                NextNavKey = Localized(next.Key);
            }
            else
            {
                NextNavKey = null!;
            }

            _isManualSelection = true;
        }

        private void ExecuteExpandSettingGroupCommand(ISettingGroupViewModel model)
        {
            _isManualSelection = false;
            if (model.IsExpanded)
            {
                _backwardNavStack.Push(Selection);
                PreviousNavKey = Localized(Selection.Key);
                Selection = model;
                model.IsExpanded = false;
            }
            else
            {
                if (_backwardNavStack.TryPop(out var previous))
                {
                    if (_backwardNavStack.TryPeek(out var previousNav))
                    {
                        PreviousNavKey = Localized(previousNav.Key);
                    }
                    else
                    {
                        PreviousNavKey = null!;
                    }
                    Selection = previous;
                }
                else
                {
                    // initial state
                    Selection = _settings.Items.OfType<ISettingGroupViewModel>().FirstOrDefault()!;
                }
            }

            _forwardNavStack.Clear();
            _isManualSelection = true;
        }

        public IEnumerable<ISettingGroupViewModel> SettingGroups { get; }

        private ISettingGroupViewModel _settings = null!;
        public ISettingGroupViewModel Settings 
        {
            get => _settings;
            private set
            {
                if (_settings != value)
                {
                    _settings = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isManualSelection = true;
        private ISettingGroupViewModel _selection = null!;
        public ISettingGroupViewModel Selection
        {
            get => _selection;
            set
            {
                if (_selection != value)
                {
                    if (_isManualSelection && _selection != null)
                    {
                        _backwardNavStack.Push(_selection);
                        _forwardNavStack.Clear();
                        PreviousNavKey = Localized(_selection.Key);
                    }

                    _selection = value;
                    OnPropertyChanged();
                }
            }
        }

        private string Localized(string key) => SettingsUI.ResourceManager.GetString($"{key}_Title") ?? $"[missing key:{key}_Title]";

        private string? _previousKey;
        public string? PreviousNavKey
        {
            get => _previousKey;
            set
            {
                if (_previousKey != value)
                {
                    _previousKey = value;
                    OnPropertyChanged();
                }
            }
        }

        private string? _nextKey;
        public string? NextNavKey
        {
            get => _nextKey;
            set
            {
                if (_nextKey != value)
                {
                    _nextKey = value;
                    OnPropertyChanged();
                }
            }
        }

        protected override void ResetToDefaults()
        {
            var settings = _service.Settings;
            if (settings.Value != settings.DefaultValue && ConfirmReset())
            {
                _service.SettingsProvider.Write(settings with { Value = settings.DefaultValue });
                _message.ShowMessage(new() { Key = $"{nameof(SettingsWindowViewModel)}.{nameof(ResetToDefaults)}.Completed", Level = LogLevel.Information, Title = "Reset Settings", Message = "All settings have been reset to their default value." });
            }
        }

        private bool ConfirmReset()
        {
            var settingsRoot = _service.Settings;
            var generalSettings = settingsRoot.GeneralSettings;
            var messageKey = "SettingsWindow_ConfirmReset";

            var model = new MessageRequestModel
            {
                Key = messageKey,
                Level = LogLevel.Warning,
                Title = "Reset Settings?",
                Message = "This will reset all settings to their default value.",
            };

            var result = _message.ShowMessageRequest(model, provider => provider.OkCancel());
            return (!result.IsEnabled && result.MessageAction == MessageAction.Undefined)
                || result.MessageAction == MessageAction.AcceptAction;
        }
    }
}
