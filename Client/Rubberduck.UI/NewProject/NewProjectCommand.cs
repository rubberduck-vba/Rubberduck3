using Rubberduck.InternalApi.Model;
using Rubberduck.UI.Command;
using Rubberduck.UI.Message;
using Rubberduck.UI.Services;
using Rubberduck.UI.Services.Abstract;
using System;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;

namespace Rubberduck.UI.NewProject
{
    public class NewProjectCommand : CommandBase
    {
        private readonly NewProjectWindowFactory _factory;
        private readonly MessageActionsProvider _actions;
        private readonly ShowRubberduckSettingsCommand _showSettingsCommand;
        private readonly IFileSystem _fileSystem;
        private readonly IMessageService _messages;
        private readonly IWorkspaceFolderService _workspaceFolderService;
        private readonly IProjectFileService _projectFileService;
        private readonly ITemplatesService _templatesService;

        public NewProjectCommand(UIServiceHelper service, NewProjectWindowFactory factory, 
            IWorkspaceFolderService workspaceFolderService, 
            IFileSystem fileSystem, IProjectFileService projectFileService,
            IMessageService messages, ITemplatesService templatesService,
            MessageActionsProvider actions, ShowRubberduckSettingsCommand showSettingsCommand) 
            : base(service)
        {
            _fileSystem = fileSystem;
            _messages = messages;
            _templatesService = templatesService;
            _workspaceFolderService = workspaceFolderService;
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

            Service.TryRunAction(() =>
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

                    var root = _fileSystem.Path.Combine(model.WorkspaceLocation, model.ProjectName);
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

                    // TODO project files created, now open them!

                }
            }, nameof(NewProjectCommand));

            await Task.CompletedTask;
        }

        private ProjectFile CreateProjectFileModel(NewProjectWindowViewModel model)
        {
            return new ProjectFileBuilder(_fileSystem, Service.SettingsProvider)
                .WithModel(model).Build();
        }
    }
}
