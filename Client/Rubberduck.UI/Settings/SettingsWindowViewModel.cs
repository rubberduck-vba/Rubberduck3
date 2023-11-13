using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Resources;
using Rubberduck.SettingsProvider;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.SettingsProvider.Model.General;
using Rubberduck.SettingsProvider.Model.LanguageClient;
using Rubberduck.SettingsProvider.Model.ServerStartup;
using Rubberduck.SettingsProvider.Model.TelemetryServer;
using Rubberduck.UI.Command;
using Rubberduck.UI.Message;
using Rubberduck.UI.Services;
using Rubberduck.UI.Settings.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;

namespace Rubberduck.UI.Settings
{
    public interface ISettingViewModelFactory
    {
        ISettingViewModel CreateViewModel(BooleanRubberduckSetting setting);
        ISettingViewModel CreateViewModel(NumericRubberduckSetting setting);
        ISettingViewModel CreateViewModel(StringRubberduckSetting setting);
        ISettingViewModel CreateViewModel(UriRubberduckSetting setting);

        ISettingViewModel CreateViewModel(LogLevelSetting setting);
        ISettingViewModel CreateViewModel(TraceLevelSetting setting);
        ISettingViewModel CreateViewModel(ServerTransportTypeSetting setting);
        ISettingViewModel CreateViewModel(ServerMessageModeSetting setting);
        ISettingViewModel CreateViewModel(TypedRubberduckSetting<TimeSpan> setting);
        ISettingViewModel CreateViewModel(TypedRubberduckSetting<string[]> setting);

        ISettingGroupViewModel CreateViewModel(TypedSettingGroup settingGroup);
    }

    public class SettingViewModelFactory : ISettingViewModelFactory
    {
        private readonly ServiceHelper _service;

        public SettingViewModelFactory(ServiceHelper service) 
        {
            _service = service;
        }

        public ISettingViewModel CreateViewModel(BooleanRubberduckSetting setting) => new BooleanSettingViewModel(setting);

        public ISettingViewModel CreateViewModel(NumericRubberduckSetting setting) => new NumericSettingViewModel(setting);

        public ISettingViewModel CreateViewModel(StringRubberduckSetting setting) => new StringSettingViewModel(setting);

        public ISettingViewModel CreateViewModel(UriRubberduckSetting setting) => new UriSettingViewModel(setting);

        public ISettingViewModel CreateViewModel(LogLevelSetting setting) => new LogLevelSettingViewModel(setting);

        public ISettingViewModel CreateViewModel(TraceLevelSetting setting) => new MessageTraceLevelSettingViewModel(setting);

        public ISettingViewModel CreateViewModel(ServerTransportTypeSetting setting) => new ServerTransportTypeSettingViewModel(setting);

        public ISettingViewModel CreateViewModel(ServerMessageModeSetting setting) => new ServerMessageModeSettingViewModel(setting);
        public ISettingViewModel CreateViewModel(TypedRubberduckSetting<TimeSpan> setting) => new TimeSpanSettingViewModel(setting);
        public ISettingViewModel CreateViewModel(TypedRubberduckSetting<string[]> setting) => new ListSettingViewModel(_service, setting);

        public ISettingGroupViewModel CreateViewModel(TypedRubberduckSetting<BooleanRubberduckSetting[]> settingGroup)
        {
            var items = settingGroup.TypedValue.Select(e => CreateViewModel(e)).ToList();
            return new SettingGroupViewModel(settingGroup, items);
        }

        public ISettingGroupViewModel CreateViewModel(TypedSettingGroup settingGroup)
        {
            if (settingGroup is RubberduckSettings root)
            {
                var items = root.TypedValue.OfType<TypedSettingGroup>().Select(e => CreateViewModel(e)).ToList();
                return new SettingGroupViewModel(root, items);
            }
            else
            {
                return new SettingGroupViewModel(settingGroup, GetChildItems(settingGroup).ToList());
            }
        }

        private IEnumerable<ISettingViewModel> GetChildItems(TypedSettingGroup settingGroup)
        {
            foreach (var setting in settingGroup.TypedValue)
            {
                yield return GetChildItem(setting);
            }
        }

        private ISettingViewModel GetChildItem(RubberduckSetting setting)
        {
            switch (setting)
            {
                case BooleanRubberduckSetting booleanSetting:
                    return CreateViewModel(booleanSetting);
                case NumericRubberduckSetting numericSetting:
                    return CreateViewModel(numericSetting);
                case StringRubberduckSetting stringSetting:
                    return CreateViewModel(stringSetting);
                case UriRubberduckSetting uriSetting:
                    return CreateViewModel(uriSetting);
                case LogLevelSetting loglevelSetting:
                    return CreateViewModel(loglevelSetting);
                case TraceLevelSetting tracelevelSetting:
                    return CreateViewModel(tracelevelSetting);
                case ServerTransportTypeSetting serverTransportTypeSetting:
                    return CreateViewModel(serverTransportTypeSetting);
                case ServerMessageModeSetting serverMessageModeSetting:
                    return CreateViewModel(serverMessageModeSetting);
                case ClientHealthCheckIntervalSetting healthCheckIntervalSetting:
                    return CreateViewModel(healthCheckIntervalSetting);
                case TypedRubberduckSetting<TimeSpan> timeSpanSetting:
                    return CreateViewModel(timeSpanSetting);
                case TypedRubberduckSetting<string[]> listSetting:
                    return new ListSettingViewModel(_service, listSetting);
                case TypedSettingGroup subGroup:
                    return CreateViewModel(subGroup);
                case TypedRubberduckSetting<BooleanRubberduckSetting[]> telemetrySettingGroup:
                    return CreateViewModel(telemetrySettingGroup);
                default:
                    Debug.WriteLine($"**BUG** Missing case for '{setting.GetType()}' in SettingViewModelFactory.");
                    throw new NotSupportedException();
            }
        }
    }

    public interface ISettingsWindowViewModel { }

    public class SettingsWindowViewModel : DialogWindowViewModel, ISettingsWindowViewModel
    {
        private readonly IMessageService _message;
        private readonly ISettingViewModelFactory _factory;
        private readonly ServiceHelper _service;

        public SettingsWindowViewModel(ServiceHelper service, MessageActionCommand[] actions, IMessageService message, ISettingViewModelFactory factory)
            : base(RubberduckUI.Settings, actions)
        {
            _message = message;
            _factory = factory;
            _service = service;
            ShowSettingsCommand = new DelegateCommand(service, parameter => ResetToDefaults());
            Settings = _factory.CreateViewModel(_service.Settings);
        }

        public ISettingGroupViewModel Settings { get; }

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
                //_service.Write(settings with { Value = settings.DefaultValue });
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
            if (!result.IsEnabled && result.MessageAction != MessageAction.Undefined)
            {
                var newValue = generalSettings.DisabledMessageKeys.Append(model.Key);
                var newSetting = generalSettings.TypedValue.OfType<DisabledMessageKeysSetting>().Single().WithValue(newValue);
                var newGeneralSettings = generalSettings.WithSetting(newSetting);
                var newRubberduckSettings = (RubberduckSettings)settingsRoot.WithSetting(newGeneralSettings);
                //_service.Settings.Write(newRubberduckSettings);
            }

            return (!result.IsEnabled && result.MessageAction == MessageAction.Undefined)
                || result.MessageAction == MessageAction.AcceptAction;
        }
    }
}
