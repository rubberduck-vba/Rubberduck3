using Rubberduck.InternalApi.Model.Workspace;
using Rubberduck.Parsing._v3;
using Rubberduck.UI.Command;
using Rubberduck.UI.Services;
using Rubberduck.UI.Services.NewProject;
using Rubberduck.Unmanaged;
using Rubberduck.Unmanaged.Abstract;
using Rubberduck.Unmanaged.TypeLibs.Abstract;
using Rubberduck.VBEditor.UI.OfficeMenus;
using Rubberduck.VBEditor.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Rubberduck.Unmanaged.UIContext;
using System.Linq;
using System.IO.Abstractions;
using Reference = Rubberduck.InternalApi.Model.Workspace.Reference;

namespace Rubberduck.Main.Commands.NewWorkspace
{
    class NewWorkspaceCommand : ComCommandBase, INewWorkspaceCommand
    {
        private readonly ICommand _command;

        public NewWorkspaceCommand(UIServiceHelper service, IVbeEvents vbeEvents,
            NewProjectCommand command)
            : base(service, vbeEvents)
        {
            _command = command;
        }

        protected async override Task OnExecuteAsync(object? parameter)
        {
            _command.Execute(parameter);
            await Task.CompletedTask;
        }
    }

    class WorkspaceModulesService : IWorkspaceModulesService
    {
        private readonly IFileSystem _fileSystem;
        private readonly IProjectsProvider _projects;
        private readonly IVBETypeLibsAPI _api;
        private readonly WorkspaceFolderMigrationService _folderMigration;
        private readonly IUiDispatcher _ui;

        public WorkspaceModulesService(IFileSystem fileSystem, IProjectsProvider projects, IVBETypeLibsAPI api, WorkspaceFolderMigrationService folderMigration, IUiDispatcher ui)
        {
            _fileSystem = fileSystem;
            _projects = projects;
            _api = api;
            _folderMigration = folderMigration;
            _ui = ui;
        }

        public void ExportWorkspaceModules(string projectId, string srcRoot, IEnumerable<Module> modules)
        {
            var workspaceModules = modules.ToDictionary(e => e.Name);
            foreach (var qualifiedComponent in _projects.Components(projectId))
            {
                var component = qualifiedComponent.Component;
                if (workspaceModules.TryGetValue(component.Name, out var module))
                {
                    var filename = module.Uri.Split(_fileSystem.Path.DirectorySeparatorChar).Last();
                    var path = srcRoot + module.Uri[..^(filename.Length + 1)];
                    component.ExportAsSourceFile(path);
                }
            }
        }

        public void ImportWorkspaceModules(string projectId, string srcRoot, IEnumerable<Module> modules)
        {
            var workspaceModules = modules.ToDictionary(e => e.Name);
            foreach (var qualifiedComponent in _projects.Components(projectId))
            {
                var component = qualifiedComponent.Component;
                var collection = component.Collection;

                if (workspaceModules.TryGetValue(component.Name, out var module))
                {
                    var path = _fileSystem.Path.Combine(srcRoot, module.Uri);
                    collection.RemoveSafely(component);
                    collection.ImportSourceFile(path);
                }
            }
        }

        public IEnumerable<Module> GetWorkspaceModules(string projectId, string srcRoot, bool scanFolderAnnotations)
        {
            foreach (var qualifiedComponent in _projects.Components(projectId))
            {
                var component = qualifiedComponent.Component;
                var module = component.CodeModule;
                var declarations = module.GetLines(1, Math.Max(1, module.CountOfDeclarationLines));

                var uri = new Uri(component.Name + component.Type.FileExtension(), UriKind.Relative);
                if (scanFolderAnnotations)
                {
                    try
                    {
                        uri = _folderMigration.ParseModuleUri(projectId, (component.SafeName ?? component.Name) + component.Type.FileExtension(), declarations);
                    }
                    catch(Exception exception)
                    {
                        // TODO log failure
                    }
                }

                DocClassType? docClassType = null;
                // FIXME somehow the ITypeLib pointer is null
                //if (component.Type == Unmanaged.Model.ComponentType.Document)
                //{
                //    try
                //    {
                //        // must access TypeLibApi from main/UI thread
                //        //_ui.Invoke(() =>
                //        //{
                //        //    docClassType = (DocClassType?)_api.DetermineDocumentClassType(component); // TODO properly migrate the enum type to a shared library
                //        //});
                //    }
                //    catch (Exception exception) 
                //    { 
                //        // TODO log failure
                //    }
                //}

                yield return new()
                {
                    Name = module.Name,
                    IsAutoOpen = false,
                    Uri = uri.OriginalString,
                    Super = docClassType
                };
            }
        }

        public IEnumerable<Reference> GetWorkspaceReferences(string projectId)
        {
            var project = _projects.Project(projectId);
            var collection = project.References;
            foreach (var reference in collection)
            {
                yield return new Reference
                {
                    Name = reference.Name,
                    Guid = string.IsNullOrWhiteSpace(reference.Guid) ? Guid.Empty : new Guid(reference.Guid),
                    Major = string.IsNullOrWhiteSpace(reference.Guid) ? null : reference.Major,
                    Minor = string.IsNullOrWhiteSpace(reference.Guid) ? null : reference.Minor,
                    IsUnremovable = reference.IsBuiltIn,
                    Uri = reference.FullPath,
                    //TypeLibInfoUri = /* TODO? */
                };
            }
        }

        public void SetProjectReferences(string projectId, IEnumerable<Reference> references)
        {
            var project = _projects.Project(projectId);
            var collection = project.References;

            var keys = collection.Select(e => e.FullPath).ToHashSet();
            foreach (var reference in references)
            {
                if (reference.Uri != null && !keys.Contains(reference.Uri))
                {
                    if (reference.Guid.HasValue && reference.Guid != Guid.Empty)
                    {
                        collection.AddFromGuid(reference.Guid.ToString(), reference.Major ?? 0, reference.Minor ?? 0);
                    }
                    else
                    {
                        collection.AddFromFile(reference.Uri);
                    }
                }
            }
        }
    }
}