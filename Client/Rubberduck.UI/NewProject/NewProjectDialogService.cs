using Microsoft.Extensions.Logging;
using Rubberduck.SettingsProvider;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.UI.Command;
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
        public NewProjectDialogService(ILogger logger, 
            IWindowFactory<NewProjectWindow, NewProjectWindowViewModel> factory, 
            RubberduckSettingsProvider settingsProvider,
            IVBProjectInfoProvider projectsProvider,
            MessageActionsProvider actionsProvider,
            ShowLanguageClientSettingsCommand showSettingsCommand) 
            : base(logger, factory, settingsProvider, actionsProvider)
        {
            _projectsProvider = projectsProvider;
            _showSettingsCommand = showSettingsCommand;
        }

        protected override NewProjectWindowViewModel CreateViewModel(RubberduckSettings settings, MessageActionsProvider actions)
        {
            var projects = _projectsProvider.GetProjectInfo();
            return new NewProjectWindowViewModel(settings, projects, actions, _showSettingsCommand);
        }
    }
}
