using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Settings.Model.LanguageClient;
using Rubberduck.Resources;
using Rubberduck.UI.Command.Abstract;
using Rubberduck.UI.Command.SharedHandlers;
using Rubberduck.UI.Shared.Message;
using Rubberduck.UI.Shared.Settings;
using Rubberduck.UI.Shared.Settings.Abstract;
using Rubberduck.UI.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Rubberduck.UI.Services.Settings
{
    public class SettingsWindowViewModel : DialogWindowViewModel, ISettingsWindowViewModel
    {
        private readonly IMessageService _message;
        private readonly ISettingViewModelFactory _factory;
        private readonly UIServiceHelper _service;

        public SettingsWindowViewModel(UIServiceHelper service, MessageActionCommand[] actions, IMessageService message, ISettingViewModelFactory factory)
            : base(service, RubberduckUI.Settings, actions)
        {
            _message = message;
            _factory = factory;
            _service = service;
            ShowSettingsCommand = new DelegateCommand(service, parameter => ResetToDefaults());
            service.RunOnMainThread(() => Settings = _factory.CreateViewModel(_service.Settings));

            ExpandSettingGroupCommand = new DelegateCommand(service, parameter =>
            {
                if (parameter is ISettingGroupViewModel model)
                {
                    ExecuteExpandSettingGroupCommand(model);
                }
            });

            CommandBindings =
            [
                new(NavigationCommands.Search, DialogCommandHandlers.BrowseLocationCommandBinding_Executed, DialogCommandHandlers.BrowseLocationCommandBinding_CanExecute),
            ];
        }

        public override IEnumerable<CommandBinding> CommandBindings { get; }

        public bool ShowPinButton => false;
        
        public ICommand ExpandSettingGroupCommand { get; }

        private Stack<ISettingViewModel> _previous = [];
        private void ExecuteExpandSettingGroupCommand(ISettingGroupViewModel model)
        {
            if (model.IsExpanded)
            {
                _previous.Push(Selection);
                Selection = model;
            }
            else
            {
                if (_previous.TryPop(out var previous))
                {
                    Selection = previous;
                }
                else
                {
                    Selection = _settings.Items.FirstOrDefault()!;
                }
            }
        }

        private ISettingGroupViewModel _settings;
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
            var messageKey = $"{nameof(SettingsWindowViewModel)}.{nameof(ConfirmReset)}";

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
