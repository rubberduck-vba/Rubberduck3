using Rubberduck.InternalApi.Model.Workspace;

namespace Rubberduck.UI.WorkspaceExplorer
{
    public interface IWorkspaceSourceFileViewModel : IWorkspaceFileViewModel
    {
        DocClassType? DocumentClassType { get; set; }
    }
}
