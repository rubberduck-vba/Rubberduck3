using Rubberduck.InternalApi.Model.Workspace;
using Rubberduck.UI.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Rubberduck.UI.WorkspaceExplorer
{
    public class WorkspaceViewModel : ViewModelBase
    {
        public static WorkspaceViewModel FromModel(ProjectFile model, IWorkspaceService service)
        {
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
                if (projectFilesByFolder.TryGetValue(folder.Uri.LocalPath, out var projectFiles) && projectFiles is not null)
                {
                    var projectFilePaths = projectFiles.Select(file => file.Uri.LocalPath).ToHashSet();
                    AddFolderFiles(service, folder, projectFiles, projectFilePaths);
                    projectFilesByFolder.Remove(folder.Uri.LocalPath);
                }
                else
                {
                    // folder is empty
                    vm.Folders.Add(folder);
                }
            }
            
            // if any keys remain, they are files/folders that do not synchronize with the VBE
            foreach (var key in projectFilesByFolder.Keys)
            {
                var folder = CreateWorkspaceFolder(service, projectFilesByFolder, key);
                vm.Folders.Add(folder);
            }

            return vm;
        }

        private static void AddFolderFiles(IWorkspaceService service, WorkspaceTreeNodeViewModel folder, IEnumerable<WorkspaceTreeNodeViewModel> projectFiles, HashSet<string> projectFilePaths)
        {
            var workspaceFiles = GetWorkspaceFilesNotInProject(service, folder, projectFilePaths);
            foreach (var file in projectFiles.Concat(workspaceFiles))
            {
                folder.ChildNodes.Add(file);
            }
        }

        private static WorkspaceTreeNodeViewModel CreateWorkspaceFolder(IWorkspaceService service, Dictionary<string, IEnumerable<WorkspaceTreeNodeViewModel>> projectFilesByFolder, string key)
        {
            var folder = new WorkspaceTreeNodeViewModel
            {
                IsInProject = true,
                Uri = new Uri(key),
                Name = service.FileSystem.Directory.GetParent(key)!.Name
            };

            var projectFiles = projectFilesByFolder[key];
            var projectFilePaths = projectFiles.Select(file => file.Uri.LocalPath).ToHashSet();

            AddFolderFiles(service, folder, projectFiles, projectFilePaths);
            return folder;
        }

        private static IEnumerable<WorkspaceFileViewModel> GetWorkspaceFilesNotInProject(IWorkspaceService service, WorkspaceTreeNodeViewModel folder, HashSet<string> projectFilePaths)
            => service.FileSystem.Directory.GetFiles(folder.Uri.LocalPath).Except(projectFilePaths)
                .Select(file => new WorkspaceFileViewModel
                {
                    Uri = new Uri(file),
                    Name = service.FileSystem.Path.GetFileNameWithoutExtension(file),
                    IsInProject = false // file exists in a project folder but is not included in the project
                });

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
