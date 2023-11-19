using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Model;
using Rubberduck.Resources.Messages;
using Rubberduck.UI.Command;
using Rubberduck.UI.Message;
using Rubberduck.UI.Services;
using Rubberduck.UI.Shell;
using System;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;

namespace Rubberduck.UI.NewProject
{


    public class OpenProjectCommand : CommandBase
    {
        private readonly IFileSystem _fileSystem;
        private readonly IProjectFileService _projectFileService;

        public OpenProjectCommand(UIServiceHelper service, 
            IProjectFileService projectFileService,
            IFileSystem fileSystem)
            : base(service)
        {
            _projectFileService = projectFileService;
            _fileSystem = fileSystem;
        }

        protected async override Task OnExecuteAsync(object? parameter)
        {
            Uri workspaceUri;
            if (parameter is null)
            {
                var prompt = new BrowseFileModel
                {
                    Title = "Open Project",
                    DefaultFileExtension = "rdproj",
                    Filter = "Rubberduck Project (.rdproj);*.rdproj",
                };
                if (!DialogCommands.BrowseFileOpen(prompt))
                {
                    throw new OperationCanceledException();
                }
                workspaceUri = new Uri(_fileSystem.Path.GetDirectoryName(prompt.Selection));
            }
            else
            {
                workspaceUri = new Uri(parameter.ToString());
            }

            var model = _projectFileService.ReadFile(workspaceUri);
            // TODO load the workspace into the editor
        }
    }

    public class NewProjectCommand : CommandBase
    {
        private readonly NewProjectWindowFactory _factory;
        private readonly MessageActionsProvider _actions;
        private readonly ShowRubberduckSettingsCommand _showSettingsCommand;
        private readonly IFileSystem _fileSystem;
        private readonly IMessageService _messages;
        private readonly IWorkspaceFolderService _workspaceFolderService;
        private readonly IProjectFileService _projectFileService;

        public NewProjectCommand(UIServiceHelper service, NewProjectWindowFactory factory, 
            IFileSystem fileSystem, IMessageService messages,
            MessageActionsProvider actions, ShowRubberduckSettingsCommand showSettingsCommand) 
            : base(service)
        {
            _fileSystem = fileSystem;
            _messages = messages;
            _factory = factory;
            _actions = actions;
            _showSettingsCommand = showSettingsCommand;
        }

        protected async override Task OnExecuteAsync(object? parameter)
        {
            // TODO get VBProjectInfo items from VBE addin client
            var projects = Enumerable.Empty<VBProjectInfo?>();
            // TODO deserialize project templates from templates folder
            var templates = new[] { ProjectTemplate.Default };

            var model = new NewProjectWindowViewModel(Service.Settings, projects, templates, _actions, _showSettingsCommand);

            Service.TryRunAction(() =>
            {
                var view = _factory.Create(model);
                if (view.ShowDialog() == true)
                {
                    _workspaceFolderService.CreateWorkspace(model.WorkspaceLocation);

                    var projectFile = CreateProjectFileModel(model);
                    _projectFileService.CreateFile(projectFile);
                }
            }, nameof(NewProjectCommand));

            await Task.CompletedTask;
        }

        private ProjectFile CreateProjectFileModel(NewProjectWindowViewModel model)
        {
            return new ProjectFileBuilder(_fileSystem, Service.SettingsProvider)
                .WithModel(model) // TODO select template in NewProject dialog
                .WithTemplate(ProjectTemplate.Default)
                .Build();
        }
    }
}
