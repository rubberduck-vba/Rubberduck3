using Microsoft.Extensions.Logging;
using Rubberduck.SettingsProvider;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.UI.Command;
using Rubberduck.UI.Message;

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
        private readonly ISettingsService<RubberduckSettings> _settingsService;
        private readonly IMessageService _messageService;
        private readonly ISettingViewModelFactory _vmFactory;
        private readonly ILogger _logger;

        public SettingsDialogService(ILogger<SettingsDialogService> logger, 
            IWindowFactory<SettingsWindow, SettingsWindowViewModel> factory, 
            IMessageService messageService,
            IRubberduckSettingsProvider settingsService,
            ISettingViewModelFactory vmFactory,
            MessageActionsProvider actionsProvider) 
            : base(logger, factory, settingsService, actionsProvider)
        {
            _logger = logger;
            _settingsService = settingsService;
            _messageService = messageService;
            _vmFactory = vmFactory;
        }

        protected override SettingsWindowViewModel CreateViewModel(RubberduckSettings settings, MessageActionsProvider actions)
        {
            var vm = new SettingsWindowViewModel(_logger, _settingsService, actions.Close(), _messageService, _vmFactory);
            return vm;
        }
    }
}
