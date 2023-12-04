using System.Collections.Generic;

namespace Rubberduck.UI.WorkspaceExplorer
{
    public interface IWorkspaceTreeNode
    {
        string Name { get; }
        IEnumerable<IWorkspaceTreeNode> Children { get; }

        bool IsSelected { get; set; }
        bool Filtered { get; set; }
        bool IsExpanded { get; set; }
    }
}
