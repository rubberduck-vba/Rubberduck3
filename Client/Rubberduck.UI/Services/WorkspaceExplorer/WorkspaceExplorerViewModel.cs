using Rubberduck.InternalApi.Model.Workspace;
using Rubberduck.UI.Services.Abstract;
using Rubberduck.UI.Windows;
using Rubberduck.UI.WorkspaceExplorer;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Rubberduck.UI.Services.WorkspaceExplorer
{
    public class WorkspaceExplorerViewModel : ViewModelBase, IWorkspaceExplorerViewModel
    {
        private readonly IWorkspaceService _service;

        public WorkspaceExplorerViewModel()
        {
            /* DESIGNER */
        }

        public WorkspaceExplorerViewModel(IWorkspaceService service)
        {
            _service = service;
            Workspaces = new(service.ProjectFiles.Select(workspace => WorkspaceViewModel.FromModel(workspace, _service)));
        }

        public string Title { get; set; } = "Workspace Explorer"; // TODO localize

        private WorkspaceTreeNodeViewModel? _selection;
        public WorkspaceTreeNodeViewModel? Selection
        {
            get => _selection;
            set
            {
                if (_selection != value)
                {
                    _selection = value;
                    OnPropertyChanged();
                    SelectionInfo = _selection;
                }
            }
        }

        private IWorkspaceUriInfo? _selectionInfo;

        public IWorkspaceUriInfo? SelectionInfo
        {
            get => _selectionInfo;
            private set
            {
                if (_selectionInfo != value)
                {
                    _selectionInfo = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(HasSelectionInfo));
                }
            }
        }

        public bool HasSelectionInfo => _selectionInfo != null;

        public ObservableCollection<WorkspaceViewModel> Workspaces { get; } = new();

        public DockingLocation DockingLocation { get; set; } = DockingLocation.DockLeft;
        public object Header
        {
            get => Title;
            set
            {
                if (Title != value?.ToString())
                {
                    Title = value?.ToString() ?? string.Empty;
                    OnPropertyChanged();
                }
            }
        }

        private object _control;
        public object Content
        {
            get => _control;
            set
            {
                if (_control != value) 
                {
                    _control = value;
                    OnPropertyChanged();
                }
            }
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

        public void Load(ProjectFile workspace)
        {
            Workspaces.Add(WorkspaceViewModel.FromModel(workspace, _service));
        }
    }
}
