using Microsoft.Extensions.Logging;
using Rubberduck.Editor.Shell;
using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.Unmanaged;
using System.Collections.Generic;

namespace Rubberduck.Editor.FileMenu
{
    public interface INewProjectDialogService : IDialogService<NewProjectWindowViewModel> 
    {
    }

    public class NewProjectDialogService : DialogService<NewProjectWindow, NewProjectWindowViewModel>
    {
        private readonly IProjectsProvider _provider;

        public NewProjectDialogService(ILogger logger, 
            IWindowFactory<NewProjectWindow, NewProjectWindowViewModel> factory, 
            ISettingsProvider<RubberduckSettings> settingsProvider, 
            MessageActionsProvider actionsProvider,
            IProjectsProvider projectsProvider) 
            : base(logger, factory, settingsProvider, actionsProvider)
        {
            _provider = projectsProvider;
        }

        protected override NewProjectWindowViewModel CreateViewModel(RubberduckSettings settings, MessageActionsProvider actions)
        {
            var projects = GetUnlockedProjects();
            return new NewProjectWindowViewModel(settings, projects, actions);
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
