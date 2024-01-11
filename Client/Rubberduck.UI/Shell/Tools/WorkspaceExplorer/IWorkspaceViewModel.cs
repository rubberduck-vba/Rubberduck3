namespace Rubberduck.UI.Shell.Tools.WorkspaceExplorer
{
    public interface IWorkspaceViewModel : IWorkspaceTreeNode
    {
        bool IsFileSystemWatcherEnabled { get; set; }
    }
}
