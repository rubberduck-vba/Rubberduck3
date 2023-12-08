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

            var srcRoot = service.FileSystem.Path.Combine(root, ProjectFile.SourceRoot);

            var vm = new WorkspaceViewModel
            {
                Name = model.VBProject.Name,
                Uri = new Uri(service.FileSystem.Path.Combine(root, ProjectFile.SourceRoot + "\\")),
                IsFileSystemWatcherEnabled = service.IsFileSystemWatcherEnabled(rootUri),
                IsExpanded = true
            };

            var sourceFiles = model.VBProject.Modules.Select(e => WorkspaceSourceFileViewModel.FromModel(e, vm.Uri) as WorkspaceTreeNodeViewModel);
            var otherFiles = model.VBProject.OtherFiles.Select(e => WorkspaceFileViewModel.FromModel(e, vm.Uri) as WorkspaceTreeNodeViewModel);
            var projectFolders = model.VBProject.Folders.Select(e => WorkspaceTreeNodeViewModel.FromModel(e, vm.Uri));

            var projectFilesByFolder = sourceFiles.Concat(otherFiles)
                .GroupBy(e => e.Uri.LocalPath[..^(service.FileSystem.Path.GetFileName(e.Uri.LocalPath).Length + 1)])
                .ToDictionary(e => e.Key, e => e.AsEnumerable());


            foreach (var folder in projectFolders.OrderBy(e => e.Name))
            {
                if (projectFilesByFolder.TryGetValue(folder.Uri.LocalPath, out var projectFiles) && projectFiles is not null)
                {
                    var projectFilePaths = projectFiles.Select(file => file.Uri.LocalPath).ToHashSet();
                    AddFolderFileNodes(service, folder, projectFiles, projectFilePaths);
                    projectFilesByFolder.Remove(folder.Uri.LocalPath);
                }
                else
                {
                    // folder is empty
                }
                vm.AddChildNode(folder);
            }

            if (projectFilesByFolder.TryGetValue(srcRoot, out var rootFolderFiles))
            {
                var rootFilePaths = rootFolderFiles.OrderBy(e => e.Name).Select(e => e.Uri.LocalPath).ToHashSet();
                AddFolderFileNodes(service, vm, rootFolderFiles, rootFilePaths);
                projectFilesByFolder.Remove(srcRoot);
            }

            foreach (var key in projectFilesByFolder.Keys)
            {
                var folder = CreateWorkspaceFolderNode(service, projectFilesByFolder[key], key);
                vm.ChildNodes.Add(folder);
            }

            return vm;
        }

        private static void AddFolderFileNodes(IWorkspaceService service, IWorkspaceTreeNode folder, IEnumerable<IWorkspaceTreeNode> projectFiles, HashSet<string> projectFilePaths)
        {
            var workspaceFiles = GetWorkspaceFilesNotInProject(service, folder, projectFilePaths);
            foreach (var file in projectFiles.Concat(workspaceFiles))
            {
                folder.AddChildNode(file);
            }
        }

        private static WorkspaceTreeNodeViewModel CreateWorkspaceFolderNode(IWorkspaceService service, IEnumerable<IWorkspaceTreeNode> projectFiles, string key)
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

        private static IEnumerable<WorkspaceFileViewModel> GetWorkspaceFilesNotInProject(IWorkspaceService service, IWorkspaceTreeNode folder, HashSet<string> projectFilePaths)
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

        private readonly ObservableCollection<IWorkspaceTreeNode> _children = new();
        public ObservableCollection<IWorkspaceTreeNode> ChildNodes => _children;

        public string Name { get; set; }
        public Uri Uri { get; set; }
        public string FileName => Uri.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped).Split('/').Last();

        public IEnumerable<IWorkspaceTreeNode> Children => _children;
        public void AddChildNode(IWorkspaceTreeNode childNode)
        {
            _children.Add(childNode);
        }

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
