using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Workspace;
using Rubberduck.InternalApi.Services;
using Rubberduck.UI.Command.Abstract;
using Rubberduck.UI.Services;
using Rubberduck.UI.Services.NewProject;
using System;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace Rubberduck.UI.Command
{
    public class NewProjectCommand : CommandBase
    {
        private readonly IFileSystem _fileSystem;
        private readonly INewProjectDialogService _dialog;

        private readonly IProjectFileService _projectFileService;
        private readonly IWorkspaceService? _workspace; // null if invoked from add-in
        private readonly IWorkspaceFolderService _workspaceFolderService;
        private readonly IWorkspaceSyncService? _workspaceModulesService; // null if invoked from add-in

        public NewProjectCommand(UIServiceHelper service,
            INewProjectDialogService dialogService,
            IWorkspaceService? workspace,
            IFileSystem fileSystem,
            IWorkspaceFolderService workspaceFolderService,
            IProjectFileService projectFileService,
            IWorkspaceSyncService? workspaceModulesService)
            : base(service)
        {
            _dialog = dialogService;

            _fileSystem = fileSystem;
            _workspaceFolderService = workspaceFolderService;
            _workspace = workspace;
            _projectFileService = projectFileService;
            _workspaceModulesService = workspaceModulesService;
        }

        protected async override Task OnExecuteAsync(object? parameter)
        {
            Uri? workspaceRootUri = null;

            if (Service.TryRunAction(() =>
            {
                if (_dialog.ShowDialog(out var model))
                {
                    // TODO actually validate the model, it's too late here.
                    if (Service.Settings.LanguageClientSettings.WorkspaceSettings.RequireDefaultWorkspaceRootHost)
                    {
                        if (model.WorkspaceLocation != Service.Settings.LanguageClientSettings.WorkspaceSettings.DefaultWorkspaceRoot.LocalPath)
                        {
                            Service.LogWarning("Cannot create workspace. Project workspace location is required to be under the default workspace root as per current configuration.");
                            throw new InvalidOperationException(); // throwing here because this should have been validated already.
                        }
                    }

                    workspaceRootUri = new Uri(_fileSystem.Path.Combine(model.WorkspaceLocation, model.ProjectName));
                    var projectFile = CreateProjectFileModel(model);

                    var workspaceSrcRoot = _fileSystem.Path.Combine(workspaceRootUri.LocalPath, WorkspaceUri.SourceRootName);
                    _workspaceFolderService.CreateWorkspaceFolders(projectFile);
                    _projectFileService.CreateFile(projectFile);

                    if (_workspaceModulesService is not null)
                    {
                        // command is executed from the VBE add-in;
                        // we're migrating an existing project to RD3 so we need to create the files now:
                        _workspaceModulesService.ExportWorkspaceModules(workspaceRootUri, projectFile.VBProject.Modules);
                    }
                    else
                    {
                        // command is executed from the Rubberduck Editor;
                        // no need to write the workspace files changes need to be saved.
                    }

                    if (model.SelectedProjectTemplate is not null)
                    {
                        var templatesRoot = _fileSystem.DirectoryInfo.New(Service.Settings.GeneralSettings.TemplatesLocation.LocalPath).FullName;
                        var templateSrcRoot = _fileSystem.Path.Combine(templatesRoot, model.SelectedProjectTemplate.Name, ProjectTemplate.TemplateSourceFolderName);

                        _workspaceFolderService.CopyTemplateFiles(model.SelectedProjectTemplate.ProjectFile, templateSrcRoot);
                    }

                    Service.LogInformation("Workspace was successfully created.", $"Workspace root: {workspaceRootUri}");
                }
            }, nameof(NewProjectCommand)) && _workspace != null && workspaceRootUri != null)
            {
                await _workspace.OpenProjectWorkspaceAsync(workspaceRootUri);
            }
        }

        private ProjectFile CreateProjectFileModel(NewProjectWindowViewModel model)
        {
            var builder = new ProjectFileBuilder(_fileSystem, Service.SettingsProvider);

            if (model.SelectedProjectTemplate is not null)
            {
                builder = builder.WithModel(model);
            }
            else if (model.SelectedVBProject is not null)
            {
                if (_workspaceModulesService is not null)
                {
                    var workspaceRoot = new Uri(_fileSystem.Path.Combine(model.WorkspaceLocation, model.ProjectName));
                    var modules = _workspaceModulesService.GetWorkspaceModules(workspaceRoot, model.ScanFolderAnnotations);
                    foreach (var module in modules)
                    {
                        builder.WithModule(module);
                    }

                    var references = _workspaceModulesService.GetWorkspaceReferences(model.SelectedVBProject.WorkspaceUri);
                    foreach (var reference in references)
                    {
                        builder.WithReference(reference);
                    }
                }
            }

            return builder.WithProjectName(model.ProjectName).Build();
        }
    }
}
