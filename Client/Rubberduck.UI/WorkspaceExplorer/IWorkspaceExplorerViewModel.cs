using Rubberduck.InternalApi.Model.Workspace;
using Rubberduck.UI.Windows;
using System.Collections.ObjectModel;

namespace Rubberduck.UI.WorkspaceExplorer
{
    public interface IWorkspaceExplorerViewModel : IToolWindowViewModel
    {
        void Load(ProjectFile workspace);
        IWorkspaceTreeNode? Selection { get; set; }
        ObservableCollection<IWorkspaceViewModel> Workspaces { get; }
    }
}
