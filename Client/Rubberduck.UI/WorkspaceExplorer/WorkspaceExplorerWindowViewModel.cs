using Rubberduck.InternalApi.Model.Workspace;
using Rubberduck.UI.Services.Abstract;
using Rubberduck.UI.Windows;
using System.Collections.ObjectModel;

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
}
