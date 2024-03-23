using Microsoft.Extensions.Logging;
using Rubberduck.UI.Shared.Settings;
using Rubberduck.UI.Shared.Message;
using Rubberduck.UI.Windows;
using System;
using System.Linq;
using Rubberduck.InternalApi.Settings;
using Rubberduck.InternalApi.Settings.Model;
using Rubberduck.InternalApi.Services;
using Rubberduck.UI.Shared.Settings.Abstract;
using Rubberduck.UI.Chrome;

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

        protected override SettingsWindowViewModel CreateViewModel(RubberduckSettings settings, MessageActionsProvider actions)
        {
            var vm = new SettingsWindowViewModel(_service, actions.OkCancel(), _chrome, _messageService, _vmFactory);
            return vm;
        }

        private ISettingGroupViewModel GetSettingGroup(string key)
        {
            var flattened = Settings.Flatten();
            
            // if the key is a top-level setting group, we return it immediately
            // because there is no need to figure out the parent.
            var groups = Settings.TypedValue.OfType<TypedSettingGroup>().ToDictionary(e => e.Key, e => e);
            if (groups.TryGetValue(key, out var settingGroup))
            {
                return _vmFactory.CreateViewModel(settingGroup);
            }

            // first find the key we're looking for
            var item = flattened.Single(e => e.Key == key);

            // now find its parent
            var parent = flattened.OfType<TypedSettingGroup>().SingleOrDefault(e => e.TypedValue.Any(e => e.Key == key));
            var parentViewModel = (ISettingGroupViewModel)CreateViewModel(Settings, _actionsProvider);

            // push parent to nav stack, then push vm
            return parentViewModel;
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

                var settingGroup = viewModel.Settings.Items.OfType<ISettingGroupViewModel>()
                    .Select(e => (VM: e, e.Key)).SingleOrDefault(e => e.Key == key).VM;

                viewModel.Selection = settingGroup ?? viewModel.Settings;

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
