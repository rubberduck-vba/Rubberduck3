using Rubberduck.UI.Services.WorkspaceExplorer;
using Rubberduck.UI.Windows;
using System.Collections.ObjectModel;

namespace Rubberduck.UI.WorkspaceExplorer
{
    public interface IWorkspaceExplorerViewModel : IToolWindowViewModel
    {
        WorkspaceTreeNodeViewModel? Selection { get; set; }
        IWorkspaceUriInfo? SelectionInfo { get; }
        bool HasSelectionInfo { get; }
        ObservableCollection<WorkspaceViewModel> Workspaces { get; }
    }
}
