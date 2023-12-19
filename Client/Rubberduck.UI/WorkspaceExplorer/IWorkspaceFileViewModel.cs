namespace Rubberduck.UI.WorkspaceExplorer
{
    public interface IWorkspaceFileViewModel : IWorkspaceTreeNode
    {
        bool IsAutoOpen { get; set; }
        bool IsReadOnly { get; set; }
    }
}
