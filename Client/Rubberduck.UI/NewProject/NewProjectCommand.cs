using Rubberduck.InternalApi.Model;
using Rubberduck.UI.Command;
using Rubberduck.UI.Command.Abstract;
using Rubberduck.UI.Message;
using Rubberduck.UI.Services;
using Rubberduck.UI.Services.Abstract;
using System;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;

namespace Rubberduck.UI.NewProject
{
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
            await Task.Yield();
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
            await Task.Yield();
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
            await Task.Yield();
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
            await Task.Yield();
            await _workspace.SaveAllAsync();
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
            if (parameter is Uri uri)
            {
                await _workspace.SaveWorkspaceFileAsync(uri);
                return;
            }

            // TODO once there's a document state manager, grab the ActiveDocument here
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
        private readonly IWorkspaceService _workspace;
        private readonly IProjectFileService _projectFileService;
        private readonly ITemplatesService _templatesService;

        public NewProjectCommand(UIServiceHelper service, NewProjectWindowFactory factory, 
            IWorkspaceFolderService workspaceFolderService, IWorkspaceService workspace,
            IFileSystem fileSystem, IProjectFileService projectFileService,
            IMessageService messages, ITemplatesService templatesService,
            MessageActionsProvider actions, ShowRubberduckSettingsCommand showSettingsCommand) 
            : base(service)
        {
            _fileSystem = fileSystem;
            _messages = messages;
            _templatesService = templatesService;
            _workspaceFolderService = workspaceFolderService;
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

            var model = new NewProjectWindowViewModel(Service.Settings, projects, templates, _actions, _showSettingsCommand);
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
