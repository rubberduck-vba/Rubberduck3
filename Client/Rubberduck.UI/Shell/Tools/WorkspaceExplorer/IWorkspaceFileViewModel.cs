namespace Rubberduck.UI.Shell.Tools.WorkspaceExplorer
{
    public interface IWorkspaceFileViewModel : IWorkspaceTreeNode
    {
        bool IsAutoOpen { get; set; }
        bool IsReadOnly { get; set; }
    }
}
