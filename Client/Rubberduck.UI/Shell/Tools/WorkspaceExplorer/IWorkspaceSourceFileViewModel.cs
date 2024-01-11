using Rubberduck.InternalApi.Model.Workspace;

namespace Rubberduck.UI.Shell.Tools.WorkspaceExplorer
{
    public interface IWorkspaceSourceFileViewModel : IWorkspaceFileViewModel
    {
        DocClassType? DocumentClassType { get; set; }
    }
}
