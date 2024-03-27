using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.InternalApi.Settings.Model;
using Rubberduck.UI.Chrome;
using Rubberduck.UI.Shared.Message;
using Rubberduck.UI.Shared.Settings;
using Rubberduck.UI.Shared.Settings.Abstract;
using Rubberduck.UI.Windows;
using System.Linq;

namespace Rubberduck.UI.Services.Settings
{
    public interface ISettingsDialogService : IDialogService<SettingsWindowViewModel>
    {
        SettingsWindowViewModel ShowDialog(string key);
    }

    public class SettingsDialogService : DialogService<SettingsWindow, SettingsWindowViewModel>, ISettingsDialogService
    {
        private readonly UIServiceHelper _service;
        private readonly IWindowChromeViewModel _chrome;
        private readonly IMessageService _messageService;
        private readonly ISettingViewModelFactory _vmFactory;
        private readonly MessageActionsProvider _actionsProvider;
        private readonly IWindowFactory<SettingsWindow, SettingsWindowViewModel> _factory;

        public SettingsDialogService(ILogger<SettingsDialogService> logger,
            RubberduckSettingsProvider settings,
            UIServiceHelper service,
            IWindowFactory<SettingsWindow, SettingsWindowViewModel> factory,
            IMessageService messageService,
            IWindowChromeViewModel chrome,
            ISettingViewModelFactory vmFactory,
            MessageActionsProvider actionsProvider,
            PerformanceRecordAggregator performance)
            : base(logger, factory, settings, actionsProvider, performance)
        {
            _service = service;
            _chrome = chrome;
            _messageService = messageService;
            _vmFactory = vmFactory;
            _actionsProvider = actionsProvider;
            _factory = factory;
        }

        protected override SettingsWindowViewModel CreateViewModel(RubberduckSettings settings, MessageActionsProvider actions) =>
            new(_service, actions.OkCancel(), _chrome, _messageService, _vmFactory);

        private SettingsWindowViewModel SetViewModelState(SettingsWindowViewModel vm, string key)
        {
            var keyItem = vm.FlattenedSettings?.FirstOrDefault(e => e.Key == key);

            if (keyItem is ISettingGroupViewModel keySettingGroup)
            {
                var parent = vm.FlattenedSettings?.OfType<ISettingGroupViewModel>()
                    .SingleOrDefault(e => e.Key == keyItem.SettingGroupKey);
                vm.Selection = parent!;

                keySettingGroup.IsExpanded = true;
                vm.Selection = keySettingGroup;
            }
            else
            {
                var settingGroup = vm.FlattenedSettings?
                        .OfType<ISettingGroupViewModel>()
                        .FirstOrDefault(e => e.Key == keyItem?.SettingGroupKey);
                vm.Selection = settingGroup ?? vm.Settings;
            }

            return vm;
        }

        public SettingsWindowViewModel ShowDialog(string key)
        {
            var viewModel = CreateViewModel(Settings, _actionsProvider);
            SetViewModelState(viewModel, key);

            var view = CreateDialog(viewModel);
            return ShowDialog(view, viewModel);
        }

        private SettingsWindow CreateDialog(SettingsWindowViewModel viewModel) => _factory.Create(viewModel);

        private SettingsWindowViewModel ShowDialog(SettingsWindow view, SettingsWindowViewModel viewModel)
        {
            if (!TryRunAction(() => view.ShowDialog(), out var exception) && exception != null)
            {
                _messageService.ShowError($"{GetType().Name}.{nameof(ShowDialog)}", exception);
            }
            else
            {
                LogDebug($"Dialog was closed. Selected action: {viewModel.SelectedAction?.ResourceKey ?? "(none)"}");
            }

            return viewModel;
        }

        protected override void OnDialogAccept(SettingsWindowViewModel model)
        {
            LogInformation($"{GetType().Name}: User accepts dialog action.");
        }

        protected override void OnDialogCancel(SettingsWindowViewModel model)
        {
            LogInformation($"{GetType().Name}: User cancels dialog action.");
            model.SelectedAction = MessageAction.CancelAction;
        }
    }
}
