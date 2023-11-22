using Rubberduck.InternalApi.Model;
using Rubberduck.UI.Services.Abstract;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Rubberduck.UI.WorkspaceExplorer
{
    public class WorkspaceExplorerWindowViewModel : ViewModelBase, IWindowViewModel
    {
        private readonly IWorkspaceService _service;
        private readonly IWorkspaceStateManager _state;
        private readonly ObservableCollection<WorkspaceViewModel> _workspaces = new();

        public WorkspaceExplorerWindowViewModel(IWorkspaceService service, IWorkspaceStateManager state)
        {
            _service = service;
            _state = state;
        }

        public string Title => "Workspace Explorer"; // TODO localize

        public ObservableCollection<WorkspaceViewModel> Workspaces => _workspaces;

        public void Load(ProjectFile workspace)
        {
            _workspaces.Add(WorkspaceViewModel.FromModel(workspace, _service));
        }
    }

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

    public class WorkspaceTreeNodeViewModel : ViewModelBase
    {
        public static WorkspaceTreeNodeViewModel FromModel(Folder model)
        {
            return new WorkspaceTreeNodeViewModel
            {
                Uri = new Uri(model.Uri),
                Name = model.Name,
                IsInProject = true
            };
        }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        private Uri _uri;
        public Uri Uri
        {
            get => _uri;
            set
            {
                if (_uri != value)
                {
                    _uri = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isInProject;
        public bool IsInProject
        {
            get => _isInProject;
            set
            {
                if (_isInProject != value)
                {
                    _isInProject = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<WorkspaceTreeNodeViewModel> _childNodes = new();
        public ObservableCollection<WorkspaceTreeNodeViewModel> ChildNodes => _childNodes;
    }

    public class WorkspaceFileViewModel : WorkspaceTreeNodeViewModel
    {
        public static WorkspaceFileViewModel FromModel(File model)
        {
            return new WorkspaceFileViewModel
            {
                Uri = new Uri(model.Uri),
                Name = model.Name,
                IsAutoOpen = model.IsAutoOpen,
                IsInProject = true
            };
        }

        private bool _isAutoOpen;
        public bool IsAutoOpen
        {
            get => _isAutoOpen;
            set
            {
                if (_isAutoOpen != value)
                {
                    _isAutoOpen = value;
                    OnPropertyChanged();
                }
            }
        }
    }

    public class WorkspaceSourceFileViewModel : WorkspaceFileViewModel
    {
        public static WorkspaceSourceFileViewModel FromModel(Module model)
        {
            return new WorkspaceSourceFileViewModel
            {
                Uri = new Uri(model.Uri),
                Name = model.Name,
                IsAutoOpen = model.IsAutoOpen,
                DocumentClassType = model.Super,
                IsInProject = true
            };
        }

        private DocClassType? _documentClassType;
        public DocClassType? DocumentClassType
        {
            get => _documentClassType;
            set
            {
                if (_documentClassType != value)
                {
                    _documentClassType = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
