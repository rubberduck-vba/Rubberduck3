using Rubberduck.Editor.DialogServices.NewProject;
using Rubberduck.InternalApi.Model.Workspace;
using Rubberduck.UI.Command;
using Rubberduck.UI.Command.Abstract;
using Rubberduck.UI.Command.SharedHandlers;
using Rubberduck.UI.Message;
using Rubberduck.UI.Services;
using Rubberduck.UI.Services.Abstract;
using System;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Rubberduck.UI.NewProject
{
    public class ExitCommand : CommandBase
    {
        private readonly IWorkspaceService _workspace;

        public ExitCommand(UIServiceHelper service, IWorkspaceService workspace)
            : base(service)
        {
            _workspace = workspace;
        }

        protected override async Task OnExecuteAsync(object? parameter)
        {
            await _workspace.SaveAllAsync();
            // TODO synchronize workspace
            Application.Current.Shutdown(); // FIXME closes the editor window, but does not exit the process...
        }
    }

    public class SynchronizeWorkspaceCommand : CommandBase
    {
        public SynchronizeWorkspaceCommand(UIServiceHelper service) 
            : base(service)
        {
            // TODO cannot execute without a connected add-in client
            //AddToCanExecuteEvaluation()
        }

        protected override Task OnExecuteAsync(object? parameter)
        {
            throw new NotImplementedException();
        }
    }

    public class CloseWorkspaceCommand : CommandBase
    {
        private readonly IWorkspaceService _workspace;

        public CloseWorkspaceCommand(UIServiceHelper service,
            IWorkspaceService workspace)
            : base(service)
        {
            _workspace = workspace;
        }

        protected async override Task OnExecuteAsync(object? parameter)
        {
            _workspace.CloseWorkspace();
        }
    }

    public class CloseAllDocumentsCommand : CommandBase
    {
        private readonly IWorkspaceService _workspace;

        public CloseAllDocumentsCommand(UIServiceHelper service,
            IWorkspaceService workspace)
            : base(service)
        {
            _workspace = workspace;
        }

        protected async override Task OnExecuteAsync(object? parameter)
        {
            _workspace.CloseAllFiles();
        }
    }

    public class CloseDocumentCommand : CommandBase
    {
        private readonly IWorkspaceService _workspace;

        public CloseDocumentCommand(UIServiceHelper service,
            IWorkspaceService workspace)
            : base(service)
        {
            _workspace = workspace;
        }

        protected async override Task OnExecuteAsync(object? parameter)
        {
            if (parameter is Uri uri)
            {
                _workspace.CloseFile(uri);
                return;
            }

            // TODO once there's a document state manager, grab the ActiveDocument here
        }
    }

    public class SaveAsProjectTemplateCommand : CommandBase
    {
        private readonly IWorkspaceService _workspace;
        private readonly ITemplatesService _templates;

        public SaveAsProjectTemplateCommand(UIServiceHelper service,
            IWorkspaceService workspace, ITemplatesService templates)
            : base(service)
        {
            _workspace = workspace;
            _templates = templates;
        }

        protected override async Task OnExecuteAsync(object? parameter)
        {
            var template = new ProjectTemplate(); // TODO ExportTemplateService
            _templates.SaveProjectTemplate(template);
        }
    }

    public class SaveAllDocumentsCommand : CommandBase
    {
        private readonly IWorkspaceService _workspace;

        public SaveAllDocumentsCommand(UIServiceHelper service,
            IWorkspaceService workspace)
            : base(service)
        {
            _workspace = workspace;
        }

        protected async override Task OnExecuteAsync(object? parameter)
        {
            await _workspace.SaveAllAsync();
        }
    }

    public class SaveDocumentAsCommand : CommandBase
    {
        private readonly IWorkspaceService _workspace;

        public SaveDocumentAsCommand(UIServiceHelper service,
            IWorkspaceService workspace)
            : base(service)
        {
            _workspace = workspace;
        }

        protected async override Task OnExecuteAsync(object? parameter)
        {
            // TODO once there's a document state manager, grab the ActiveDocument here
            // TODO prompt for file name
            //_workspace.SaveWorkspaceFileAsync(uri);
        }
    }
    public class SaveDocumentCommand : CommandBase
    {
        private readonly IWorkspaceService _workspace;

        public SaveDocumentCommand(UIServiceHelper service,
            IWorkspaceService workspace)
            : base(service)
        {
            _workspace = workspace;
        }

        protected async override Task OnExecuteAsync(object? parameter)
        {
            // TODO once there's a document state manager, grab the ActiveDocument here
            //_workspace.SaveWorkspaceFileAsync(uri);
        }
    }

    public class NewProjectCommand : CommandBase
    {
        private readonly NewProjectWindowFactory _factory;
        private readonly MessageActionsProvider _actions;
        private readonly ShowRubberduckSettingsCommand _showSettingsCommand;
        private readonly SystemCommandHandlers _systemCommandHandlers;
        private readonly IFileSystem _fileSystem;
        private readonly IMessageService _messages;
        private readonly IWorkspaceFolderService _workspaceFolderService;
        private readonly IWorkspaceService _workspace;
        private readonly IProjectFileService _projectFileService;
        private readonly ITemplatesService _templatesService;

        public NewProjectCommand(UIServiceHelper service, NewProjectWindowFactory factory, 
            IWorkspaceFolderService workspaceFolderService, IWorkspaceService workspace,
            IFileSystem fileSystem, IProjectFileService projectFileService,
            IMessageService messages, ITemplatesService templatesService, SystemCommandHandlers systemCommandHandlers,
            MessageActionsProvider actions, ShowRubberduckSettingsCommand showSettingsCommand) 
            : base(service)
        {
            _fileSystem = fileSystem;
            _messages = messages;
            _templatesService = templatesService;
            _workspaceFolderService = workspaceFolderService;
            _systemCommandHandlers = systemCommandHandlers;
            _workspace = workspace;
            _projectFileService = projectFileService;
            _factory = factory;
            _actions = actions;
            _showSettingsCommand = showSettingsCommand;
        }

        protected async override Task OnExecuteAsync(object? parameter)
        {
            // TODO get VBProjectInfo items from VBE addin client
            var projects = Enumerable.Empty<VBProjectInfo?>();
            var templates = _templatesService.GetProjectTemplates();

            var model = new NewProjectWindowViewModel(Service, projects, templates, _actions, _showSettingsCommand, _systemCommandHandlers);
            string? root = default;

            if (Service.TryRunAction(() =>
            {
                var view = _factory.Create(model);
                if (view.ShowDialog() == true)
                {
                    if (Service.Settings.LanguageClientSettings.WorkspaceSettings.RequireDefaultWorkspaceRootHost)
                    {
                        if (model.WorkspaceLocation != Service.Settings.LanguageClientSettings.WorkspaceSettings.DefaultWorkspaceRoot.LocalPath)
                        {
                            Service.LogWarning("Cannot create workspace. Project workspace location is required to be under the default workspace root as per current configuration.");
                            throw new InvalidOperationException();
                        }
                    }

                    root = _fileSystem.Path.Combine(model.WorkspaceLocation, model.ProjectName);
                    var projectFile = CreateProjectFileModel(model);

                    var workspaceSrcRoot = _fileSystem.Path.Combine(root, ProjectFile.SourceRoot);
                    _workspaceFolderService.CreateWorkspaceFolders(projectFile, root);
                    _projectFileService.CreateFile(projectFile);

                    if (model.SelectedProjectTemplate is not null)
                    {
                        var template = _templatesService.Resolve(model.SelectedProjectTemplate);
                        var templateName = template.Name;
                        var templatesRoot = _fileSystem.DirectoryInfo.New(Service.Settings.GeneralSettings.TemplatesLocation.LocalPath).FullName;
                        var templateSrcRoot = _fileSystem.Path.Combine(templatesRoot, templateName, ProjectTemplate.TemplateSourceFolderName);
                        _workspaceFolderService.CopyTemplateFiles(template.ProjectFile, workspaceSrcRoot, templateSrcRoot);
                    }

                    Service.LogInformation("Workspace was successfully created.", $"Workspace root: {root}");
                }
            }, nameof(NewProjectCommand)) && root != default)
            {
                await _workspace.OpenProjectWorkspaceAsync(new Uri(root));
            }
        }

        private ProjectFile CreateProjectFileModel(NewProjectWindowViewModel model)
        {
            return new ProjectFileBuilder(_fileSystem, Service.SettingsProvider)
                .WithModel(model).Build();
        }
    }
}
