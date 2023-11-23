using Microsoft.Extensions.Logging;
using Rubberduck.SettingsProvider;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.UI.Command;
using Rubberduck.UI.Services.Abstract;
using System.Collections.Generic;
using System.Windows.Input;

namespace Rubberduck.UI.NewProject
{

    public interface IVBProjectInfoProvider
    {
        IEnumerable<VBProjectInfo?> GetProjectInfo();
    }

    public interface INewProjectDialogService : IDialogService<NewProjectWindowViewModel> 
    {
    }

    public class NewProjectDialogService : DialogService<NewProjectWindow, NewProjectWindowViewModel>, INewProjectDialogService
    {
        private readonly ICommand _showSettingsCommand;
        private readonly IVBProjectInfoProvider _projectsProvider;
        private readonly ITemplatesService _templatesService;

        public NewProjectDialogService(ILogger logger, 
            IWindowFactory<NewProjectWindow, NewProjectWindowViewModel> factory, 
            RubberduckSettingsProvider settingsProvider,
            IVBProjectInfoProvider projectsProvider,
            ITemplatesService templatesService,
            MessageActionsProvider actionsProvider,
            ShowRubberduckSettingsCommand showSettingsCommand,
            PerformanceRecordAggregator performance) 
            : base(logger, factory, settingsProvider, actionsProvider, performance)
        {
            _projectsProvider = projectsProvider;
            _showSettingsCommand = showSettingsCommand;
            _templatesService = templatesService;
        }

        protected override NewProjectWindowViewModel CreateViewModel(RubberduckSettings settings, MessageActionsProvider actions)
        {
            var projects = _projectsProvider.GetProjectInfo();
            var templates = _templatesService.GetProjectTemplates();
            return new NewProjectWindowViewModel(settings, projects, templates, actions, _showSettingsCommand);
        }
    }
}
