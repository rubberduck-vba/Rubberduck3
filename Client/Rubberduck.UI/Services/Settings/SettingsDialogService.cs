using Microsoft.Extensions.Logging;
using Rubberduck.UI.Shared.Settings;
using Rubberduck.UI.Shared.Message;
using Rubberduck.UI.Windows;
using System;
using System.Linq;
using Rubberduck.InternalApi.Settings;
using Rubberduck.InternalApi.Settings.Model;
using Rubberduck.InternalApi.Services;

namespace Rubberduck.UI.Services.Settings
{
    public interface ISettingsDialogService : IDialogService<SettingsWindowViewModel>
    {
        SettingsWindowViewModel ShowDialog(string key);
    }

    public class SettingsDialogService : DialogService<SettingsWindow, SettingsWindowViewModel>, ISettingsDialogService
    {
        private readonly UIServiceHelper _service;
        private readonly IMessageService _messageService;
        private readonly ISettingViewModelFactory _vmFactory;
        private readonly MessageActionsProvider _actionsProvider;
        private readonly IWindowFactory<SettingsWindow, SettingsWindowViewModel> _factory;

        public SettingsDialogService(ILogger<SettingsDialogService> logger,
            RubberduckSettingsProvider settings,
            UIServiceHelper service,
            IWindowFactory<SettingsWindow, SettingsWindowViewModel> factory,
            IMessageService messageService,
            ISettingViewModelFactory vmFactory,
            MessageActionsProvider actionsProvider,
            PerformanceRecordAggregator performance)
            : base(logger, factory, settings, actionsProvider, performance)
        {
            _service = service;
            _messageService = messageService;
            _vmFactory = vmFactory;
            _actionsProvider = actionsProvider;
            _factory = factory;
        }

        protected override SettingsWindowViewModel CreateViewModel(RubberduckSettings settings, MessageActionsProvider actions)
        {
            var vm = new SettingsWindowViewModel(_service, actions.Close(), _messageService, _vmFactory);
            return vm;
        }

        public SettingsWindowViewModel ShowDialog(string key)
        {
            SettingsWindowViewModel viewModel = default!;
            SettingsWindow view = default!;

            var actions = _actionsProvider;
            var verbosity = TraceLevel;

            if (TryRunAction(() =>
            {
                viewModel = CreateViewModel(Settings, actions)
                    ?? throw new InvalidOperationException($"CreateViewModel returned null.");

                var settingGroup = viewModel.Settings.Items.Select(e => (VM: e, e.Key)).SingleOrDefault(e => e.Key == key).VM;
                viewModel.Selection = settingGroup;

                view = _factory.Create(viewModel)
                    ?? throw new InvalidOperationException($"ViewFactory.Create returned null.");
            }))
            {
                TryRunAction(() => view.ShowDialog());
                return viewModel;
            }

            throw new InvalidOperationException();
        }
    }
}
