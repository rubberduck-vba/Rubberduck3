using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.UI.Command;
using Rubberduck.UI.Message;
using Rubberduck.UI.Services;
using System.Windows.Forms;
using System;
using System.Linq;
using Rubberduck.UI.Settings.ViewModels;

namespace Rubberduck.UI.Settings
{
    public class SettingsWindowFactory : IWindowFactory<SettingsWindow, SettingsWindowViewModel>
    {
        public SettingsWindow Create(SettingsWindowViewModel model) => new(model);
    }

    public interface ISettingsDialogService : IDialogService<SettingsWindowViewModel>
    {
        SettingsWindowViewModel ShowDialog(string key);
    }

    public class SettingsDialogService : DialogService<SettingsWindow, SettingsWindowViewModel>, ISettingsDialogService
    {
        private readonly UIServiceHelper _service;
        private readonly IMessageService _messageService;
        private readonly ISettingViewModelFactory _vmFactory;
        private readonly ILogger _logger;

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
                    ?? throw new ArgumentNullException(nameof(viewModel), $"CreateViewModel returned null.");

                var settingGroup = viewModel.Settings.Items.Select(e => (VM:e, Key:e.Key)).SingleOrDefault(e => e.Key == key).VM;
                viewModel.Selection = settingGroup;

                view = _factory.Create(viewModel)
                    ?? throw new ArgumentNullException(nameof(view), $"ViewFactory.Create returned null.");
            }))
            {
                TryRunAction(() => view.ShowDialog());
                return viewModel;
            }

            throw new InvalidOperationException();

        }
    }
}
