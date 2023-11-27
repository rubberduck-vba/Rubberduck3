using Rubberduck.InternalApi.Model.Workspace;
using Rubberduck.UI.Services.Abstract;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Rubberduck.UI.WorkspaceExplorer
{
    public class WorkspaceViewModel : ViewModelBase
    {
        public static WorkspaceViewModel FromModel(ProjectFile model, IWorkspaceService service)
        {
            /*TODO refactor*/

            var vm = new WorkspaceViewModel
            {
                IsFileSystemWatcherEnabled = service.IsFileSystemWatcherEnabled(model.Uri),
            };

            var sourceFiles = model.VBProject.Modules.Select(e => WorkspaceSourceFileViewModel.FromModel(e) as WorkspaceTreeNodeViewModel);
            var otherFiles = model.VBProject.OtherFiles.Select(e => WorkspaceFileViewModel.FromModel(e) as WorkspaceTreeNodeViewModel);
            var projectFolders = model.VBProject.Folders.Select(e => WorkspaceTreeNodeViewModel.FromModel(e));

            var projectFilesByFolder = sourceFiles.Concat(otherFiles)
                .GroupBy(e => service.FileSystem.Path.GetDirectoryName(e.Uri.LocalPath)!)
                .ToDictionary(e => e.Key, e => e.AsEnumerable());

            foreach (var folder in projectFolders)
            {
                if (projectFilesByFolder.TryGetValue(folder.Uri.LocalPath, out var projectFiles))
                {
                    var workspaceFiles = service.FileSystem.Directory.GetFiles(folder.Uri.LocalPath)
                        .Except(projectFiles.Select(file => file.Uri.LocalPath))
                        .Select(file => new WorkspaceFileViewModel
                        {
                            Uri = new Uri(file),
                            Name = service.FileSystem.Path.GetFileNameWithoutExtension(file),
                            IsInProject = false
                        }).ToList();

                    foreach (var file in projectFiles.Concat(workspaceFiles))
                    {
                        folder.ChildNodes.Add(file);
                    }

                    // remove processed keys to help next iterations
                    projectFilesByFolder.Remove(folder.Uri.LocalPath);
                }
                else
                {
                    // folder is empty
                    vm.Folders.Add(folder);
                }
            }
            
            foreach (var key in projectFilesByFolder.Keys)
            {
                // folder is not in project
                var folder = new WorkspaceTreeNodeViewModel
                {
                    IsInProject = false,
                    Uri = new Uri(key),
                    Name = service.FileSystem.Directory.GetParent(key)!.Name
                };

                var projectFiles = projectFilesByFolder[key];
                var workspaceFiles = service.FileSystem.Directory.GetFiles(folder.Uri.LocalPath)
                    .Except(projectFiles.Select(file => file.Uri.LocalPath))
                    .Select(file => new WorkspaceFileViewModel
                    {
                        Uri = new Uri(file),
                        Name = service.FileSystem.Path.GetFileNameWithoutExtension(file),
                        IsInProject = false
                    }).ToList();

                foreach (var file in projectFiles.Concat(workspaceFiles))
                {
                    // if the file is in the project, the folder is as well.. this should not happen.
                    folder.IsInProject = folder.IsInProject || file.IsInProject;
                    folder.ChildNodes.Add(file);
                }

                vm.Folders.Add(folder);
            }

            return vm;
        }

        private bool _isFileSystemWatcherEnabled;
        public bool IsFileSystemWatcherEnabled
        {
            get => _isFileSystemWatcherEnabled;
            set
            {
                if (_isFileSystemWatcherEnabled != value)
                {
                    _isFileSystemWatcherEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        private readonly ObservableCollection<WorkspaceTreeNodeViewModel> _folders = new();
        public ObservableCollection<WorkspaceTreeNodeViewModel> Folders => _folders;
    }
}
