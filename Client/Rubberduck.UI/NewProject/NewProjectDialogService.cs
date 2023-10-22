using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.UI.Command;
using Rubberduck.Unmanaged;
using System.Collections.Generic;
using System.Windows.Input;

namespace Rubberduck.UI.NewProject
{
    public interface INewProjectDialogService : IDialogService<NewProjectWindowViewModel> 
    {
    }

    public class NewProjectDialogService : DialogService<NewProjectWindow, NewProjectWindowViewModel>
    {
        private readonly IProjectsProvider _provider;
        private readonly ICommand _showSettingsCommand;

        public NewProjectDialogService(ILogger logger, 
            IWindowFactory<NewProjectWindow, NewProjectWindowViewModel> factory, 
            ISettingsProvider<RubberduckSettings> settingsProvider,
            IProjectsProvider projectsProvider,
            MessageActionsProvider actionsProvider,
            ShowLanguageClientSettingsCommand showSettingsCommand) 
            : base(logger, factory, settingsProvider, actionsProvider)
        {
            _provider = projectsProvider;
            _showSettingsCommand = showSettingsCommand;
        }

        protected override NewProjectWindowViewModel CreateViewModel(RubberduckSettings settings, MessageActionsProvider actions)
        {
            var projects = GetUnlockedProjects();
            return new NewProjectWindowViewModel(settings, projects, actions, _showSettingsCommand);
        }

        private IEnumerable<VBProjectInfo?> GetUnlockedProjects()
        {
            var result = new HashSet<VBProjectInfo?>();

            var projects = _provider.Projects();
            foreach (var wrapper in projects)
            {
                using var project = wrapper.Project;
                result.Add(new VBProjectInfo
                {
                    ProjectId = project.ProjectId,
                    Name = project.Name,
                    Location = project.FileName,
                });
            }

            return result;
        }
    }
}
