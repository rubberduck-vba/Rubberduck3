using Rubberduck.InternalApi.Model.Workspace;
using Rubberduck.UI.Services.Abstract;
using Rubberduck.UI.WorkspaceExplorer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Rubberduck.UI.Services.WorkspaceExplorer
{
    public class WorkspaceViewModel : ViewModelBase, IWorkspaceTreeNode
    {
        public static WorkspaceViewModel FromModel(ProjectFile model, IWorkspaceService service)
        {
            var root = model.Uri.LocalPath[..^(ProjectFile.FileName.Length + 1)];
            var rootUri = new Uri(root);

            var vm = new WorkspaceViewModel
            {
                Name = model.VBProject.Name,
                IsFileSystemWatcherEnabled = service.IsFileSystemWatcherEnabled(rootUri),
            };

            var sourceFiles = model.VBProject.Modules.Select(e => WorkspaceSourceFileViewModel.FromModel(e) as WorkspaceTreeNodeViewModel);
            var otherFiles = model.VBProject.OtherFiles.Select(e => WorkspaceFileViewModel.FromModel(e) as WorkspaceTreeNodeViewModel);
            var projectFolders = model.VBProject.Folders.Select(e => WorkspaceTreeNodeViewModel.FromModel(e));

            var projectFilesByFolder = sourceFiles.Concat(otherFiles)
                .GroupBy(e => service.FileSystem.Path.GetDirectoryName(e.Uri.ToString())!)
                .ToDictionary(e => service.FileSystem.Path.Combine(root, ProjectFile.SourceRoot, e.Key), e => e.AsEnumerable());

            foreach (var folder in projectFolders)
            {
                if (projectFilesByFolder.TryGetValue(folder.Uri.ToString(), out var projectFiles) && projectFiles is not null)
                {
                    var projectFilePaths = projectFiles.Select(file => file.Uri.LocalPath).ToHashSet();
                    AddFolderFileNodes(service, folder, projectFiles, projectFilePaths);
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
                var folder = CreateWorkspaceFolderNode(service, projectFilesByFolder[key], key);
                vm.Folders.Add(folder);
            }

            return vm;
        }

        private static void AddFolderFileNodes(IWorkspaceService service, WorkspaceTreeNodeViewModel folder, IEnumerable<WorkspaceTreeNodeViewModel> projectFiles, HashSet<string> projectFilePaths)
        {
            var workspaceFiles = GetWorkspaceFilesNotInProject(service, folder, projectFilePaths);
            foreach (var file in projectFiles.Concat(workspaceFiles))
            {
                folder.ChildNodes.Add(file);
            }
        }

        private static WorkspaceTreeNodeViewModel CreateWorkspaceFolderNode(IWorkspaceService service, IEnumerable<WorkspaceTreeNodeViewModel> projectFiles, string key)
        {
            var folder = new WorkspaceTreeNodeViewModel
            {
                IsInProject = true,
                Uri = new Uri(key),
                Name = service.FileSystem.DirectoryInfo.New(key).Name
            };

            var projectFilePaths = projectFiles.Select(file => service.FileSystem.Path.Combine(key, file.Uri.ToString())).ToHashSet();
            AddFolderFileNodes(service, folder, projectFiles, projectFilePaths);
            return folder;
        }

        private static IEnumerable<WorkspaceFileViewModel> GetWorkspaceFilesNotInProject(IWorkspaceService service, WorkspaceTreeNodeViewModel folder, HashSet<string> projectFilePaths)
        {
            var results = service.FileSystem.Directory.GetFiles(folder.Uri.LocalPath).Except(projectFilePaths)
                        .Select(file => new WorkspaceFileViewModel
                        {
                            Uri = new Uri(folder.Uri, file),
                            Name = service.FileSystem.Path.GetFileNameWithoutExtension(file),
                            IsInProject = false // file exists in a project folder but is not included in the project
                        });
            return results;
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

        public string Name { get; set; }

        public IEnumerable<IWorkspaceTreeNode> Children => _folders;

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isFiltered;
        public bool Filtered
        {
            get => _isFiltered;
            set
            {
                if (_isFiltered != value)
                {
                    _isFiltered = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
