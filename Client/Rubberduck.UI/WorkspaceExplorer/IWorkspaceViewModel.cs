namespace Rubberduck.UI.WorkspaceExplorer
{
    public interface IWorkspaceViewModel : IWorkspaceTreeNode
    {
        bool IsFileSystemWatcherEnabled { get; set; }
    }
}
