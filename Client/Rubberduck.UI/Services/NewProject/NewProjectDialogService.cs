using Microsoft.Extensions.Logging;
using Rubberduck.SettingsProvider;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.UI.Command.SharedHandlers;
using Rubberduck.UI.Services.Abstract;
using Rubberduck.UI.Shared.Message;
using Rubberduck.UI.Shared.NewProject;
using Rubberduck.UI.Windows;
using System.Windows.Input;

namespace Rubberduck.UI.Services.NewProject
{
    public class NewProjectDialogService : DialogService<NewProjectWindow, NewProjectWindowViewModel>, INewProjectDialogService
    {
        private readonly ICommand _showSettingsCommand;
        private readonly IVBProjectInfoProvider? _projectsProvider;
        private readonly ITemplatesService _templatesService;
        private readonly UIServiceHelper _service;

        public NewProjectDialogService(ILogger<NewProjectDialogService> logger,
            UIServiceHelper service,
            IWindowFactory<NewProjectWindow, NewProjectWindowViewModel> factory,
            RubberduckSettingsProvider settingsProvider,
            IVBProjectInfoProvider? projectsProvider,
            ITemplatesService templatesService,
            MessageActionsProvider actionsProvider,
            ShowRubberduckSettingsCommand showSettingsCommand,
            PerformanceRecordAggregator performance)
            : base(logger, factory, settingsProvider, actionsProvider, performance)
        {
            _service = service;
            _projectsProvider = projectsProvider;
            _showSettingsCommand = showSettingsCommand;
            _templatesService = templatesService;
        }

        protected override NewProjectWindowViewModel CreateViewModel(RubberduckSettings settings, MessageActionsProvider actions)
        {
            var projects = _projectsProvider?.GetProjectInfo() ?? [];
            var templates = _templatesService.GetProjectTemplates();
            return new NewProjectWindowViewModel(_service, projects, templates, actions, _showSettingsCommand);
        }

        protected override void OnDialogAccept(NewProjectWindowViewModel model)
        {
            if (model.SelectedProjectTemplate != null)
            {
                model.SelectedProjectTemplate = _templatesService.Resolve(model.SelectedProjectTemplate);
            }
        }
    }
}
