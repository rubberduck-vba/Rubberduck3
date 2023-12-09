using Rubberduck.InternalApi.Model.Workspace;
using Rubberduck.UI.Services.Abstract;
using Rubberduck.UI.Windows;
using Rubberduck.UI.WorkspaceExplorer;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Rubberduck.UI.Services.WorkspaceExplorer
{
    public class WorkspaceExplorerWindowViewModel : ViewModelBase, IWorkspaceExplorerViewModel
    {
        private readonly IWorkspaceService _service;
        private readonly ObservableCollection<WorkspaceViewModel> _workspaces = new();

        public WorkspaceExplorerWindowViewModel()
        {
            /* DESIGNER */
        }

        public WorkspaceExplorerWindowViewModel(IWorkspaceService service)
        {
            _service = service;
            _workspaces = new(service.ProjectFiles.Select(workspace => WorkspaceViewModel.FromModel(workspace, _service)));
        }

        public string Title => "Workspace Explorer"; // TODO localize

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

        public ICollection<WorkspaceViewModel> Workspaces => _workspaces;

        public DockingLocation DockingLocation { get; set; } = DockingLocation.DockLeft;

        public void Load(ProjectFile workspace)
        {
            _workspaces.Add(WorkspaceViewModel.FromModel(workspace, _service));
        }
    }
}
