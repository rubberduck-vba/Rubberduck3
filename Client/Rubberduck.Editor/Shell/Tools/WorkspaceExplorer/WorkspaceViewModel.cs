using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Workspace;
using Rubberduck.UI;
using Rubberduck.UI.Services.Abstract;
using Rubberduck.UI.WorkspaceExplorer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Rubberduck.Editor.Shell.Tools.WorkspaceExplorer
{
    public class WorkspaceViewModel : ViewModelBase, IWorkspaceTreeNode, IWorkspaceViewModel
    {
        public static WorkspaceViewModel FromModel(ProjectFile model, IWorkspaceService service)
        {
            var vm = new WorkspaceViewModel
            {
                Name = model.VBProject.Name,
                Uri = new Uri(service.FileSystem.Path.Combine(model.Uri.LocalPath, ProjectFile.SourceRoot + "\\")),
                IsFileSystemWatcherEnabled = service.IsFileSystemWatcherEnabled(model.Uri),
                IsExpanded = true
            };

            var sourceFiles = model.VBProject.Modules.Select(e => WorkspaceSourceFileViewModel.FromModel(e, model.Uri) as WorkspaceTreeNodeViewModel);
            var otherFiles = model.VBProject.OtherFiles.Select(e => WorkspaceFileViewModel.FromModel(e, model.Uri) as WorkspaceTreeNodeViewModel);
            var projectFolders = model.VBProject.Folders.Select(e => WorkspaceFolderViewModel.FromModel(e, model.Uri));

            var projectFilesByFolder = sourceFiles.Concat(otherFiles)
                .GroupBy(e => ((WorkspaceFileUri)e.Uri).WorkspaceFolder)
                .ToDictionary(e => e.Key.AbsoluteLocation.LocalPath, e => e.AsEnumerable());

            foreach (var folder in projectFolders.OrderBy(e => e.Name))
            {
                if (projectFilesByFolder.TryGetValue(((WorkspaceFolderUri)folder.Uri).AbsoluteLocation.LocalPath, out var projectFiles) && projectFiles is not null)
                {
                    var projectFilePaths = projectFiles.Select(file => ((WorkspaceFileUri)file.Uri).AbsoluteLocation.LocalPath).ToHashSet();
                    AddFolderFileNodes(service, folder, projectFiles, projectFilePaths);

                    projectFilesByFolder.Remove(((WorkspaceFolderUri)folder.Uri).AbsoluteLocation.LocalPath);
                }
                else
                {
                    // folder is empty
                }
                vm.AddChildNode(folder);
            }

            var srcRoot = new WorkspaceFolderUri(ProjectFile.SourceRoot, model.Uri);
            if (projectFilesByFolder.TryGetValue(srcRoot.AbsoluteLocation.LocalPath, out var rootFolderFiles))
            {
                var rootFilePaths = rootFolderFiles.OrderBy(e => e.Name).Select(e => e.Uri.LocalPath).ToHashSet();
                AddFolderFileNodes(service, vm, rootFolderFiles, rootFilePaths);

                projectFilesByFolder.Remove(srcRoot.AbsoluteLocation.LocalPath);
            }

            foreach (var key in projectFilesByFolder.Keys)
            {
                var folder = CreateWorkspaceFolderNode(service, projectFilesByFolder[key], new WorkspaceFolderUri(key, model.Uri));
                vm.Children.Add(folder);
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
        private static WorkspaceFolderViewModel CreateWorkspaceFolderNode(IWorkspaceService service, IEnumerable<IWorkspaceTreeNode> projectFiles, WorkspaceFolderUri uri)
        {
            var folder = new WorkspaceFolderViewModel
            {
                IsInProject = true,
                Uri = uri,
                Name = uri.AbsoluteLocation.AbsolutePath.Split('/')[^1]
            };

            var projectFilePaths = projectFiles.Select(file => service.FileSystem.Path.Combine(uri.AbsoluteLocation.LocalPath, file.Uri.ToString())).ToHashSet();
            AddFolderFileNodes(service, folder, projectFiles, projectFilePaths);
            return folder;
        }
        private static IEnumerable<WorkspaceFileViewModel> GetWorkspaceFilesNotInProject(IWorkspaceService service, IWorkspaceTreeNode folder, HashSet<string> projectFilePaths)
        {
            var workspaceRoot = ((WorkspaceFolderUri)folder.Uri).WorkspaceRoot;
            var results = service.FileSystem.Directory.GetFiles(((WorkspaceFolderUri)folder.Uri).AbsoluteLocation.LocalPath).Except(projectFilePaths)
                        .Select(file => new WorkspaceFileViewModel
                        {
                            Uri = new WorkspaceFileUri(file[workspaceRoot.LocalPath.Length..], workspaceRoot),
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
        public ObservableCollection<IWorkspaceTreeNode> Children => _children;

        public string Name { get; set; } = string.Empty;
        public Uri Uri { get; set; } = default!; // FIXME this will come back to bite me...
        public string FileName => Uri.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped).Split('/').Last();

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
