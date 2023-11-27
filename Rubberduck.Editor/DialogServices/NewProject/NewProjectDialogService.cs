using Microsoft.Extensions.Logging;
using Rubberduck.SettingsProvider;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.UI.Command;
using Rubberduck.UI.Command.SharedHandlers;
using Rubberduck.UI.NewProject;
using Rubberduck.UI.Services;
using Rubberduck.UI.Services.Abstract;
using Rubberduck.UI.Windows;
using System.Windows.Input;

namespace Rubberduck.Editor.DialogServices.NewProject
{
    public class NewProjectDialogService : DialogService<NewProjectWindow, NewProjectWindowViewModel>, INewProjectDialogService
    {
        private readonly ICommand _showSettingsCommand;
        private readonly SystemCommandHandlers _systemCommandHandlers;
        private readonly IVBProjectInfoProvider _projectsProvider;
        private readonly ITemplatesService _templatesService;
        private readonly UIServiceHelper _service;

        public NewProjectDialogService(ILogger<NewProjectDialogService> logger,
            UIServiceHelper service,
            IWindowFactory<NewProjectWindow, NewProjectWindowViewModel> factory,
            RubberduckSettingsProvider settingsProvider,
            IVBProjectInfoProvider projectsProvider,
            ITemplatesService templatesService,
            MessageActionsProvider actionsProvider,
            ShowRubberduckSettingsCommand showSettingsCommand,
            PerformanceRecordAggregator performance,
            SystemCommandHandlers systemCommandHandlers)
            : base(logger, factory, settingsProvider, actionsProvider, performance)
        {
            _service = service;
            _projectsProvider = projectsProvider;
            _showSettingsCommand = showSettingsCommand;
            _systemCommandHandlers = systemCommandHandlers;
            _templatesService = templatesService;
        }

        protected override NewProjectWindowViewModel CreateViewModel(RubberduckSettings settings, MessageActionsProvider actions)
        {
            var projects = _projectsProvider.GetProjectInfo();
            var templates = _templatesService.GetProjectTemplates();
            return new NewProjectWindowViewModel(_service, projects, templates, actions, _showSettingsCommand, _systemCommandHandlers);
        }
    }
}
