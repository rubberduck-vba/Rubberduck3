using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.UI.Command;
using Rubberduck.UI.Message;
using Rubberduck.UI.Services;

namespace Rubberduck.UI.Settings
{
    public class SettingsWindowFactory : IWindowFactory<SettingsWindow, SettingsWindowViewModel>
    {
        public SettingsWindow Create(SettingsWindowViewModel model) => new(model);
    }

    public interface ISettingsDialogService : IDialogService<SettingsWindowViewModel>
    {
    }

    public class SettingsDialogService : DialogService<SettingsWindow, SettingsWindowViewModel>, ISettingsDialogService
    {
        private readonly ServiceHelper _service;
        private readonly IMessageService _messageService;
        private readonly ISettingViewModelFactory _vmFactory;
        private readonly ILogger _logger;

        public SettingsDialogService(ILogger<SettingsDialogService> logger,
            RubberduckSettingsProvider settings,
            ServiceHelper service,
            IWindowFactory<SettingsWindow, SettingsWindowViewModel> factory, 
            IMessageService messageService,
            ISettingViewModelFactory vmFactory,
            MessageActionsProvider actionsProvider) 
            : base(logger, factory, settings, actionsProvider)
        {
            _service = service;
            _messageService = messageService;
            _vmFactory = vmFactory;
        }

        protected override SettingsWindowViewModel CreateViewModel(RubberduckSettings settings, MessageActionsProvider actions)
        {
            var vm = new SettingsWindowViewModel(_service, actions.Close(), _messageService, _vmFactory);
            return vm;
        }
    }
}
