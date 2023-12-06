using System;
using System.Collections.Generic;

namespace Rubberduck.UI.WorkspaceExplorer
{
    public interface IWorkspaceTreeNode
    {
        Uri Uri { get; }
        string Name { get; }
        IEnumerable<IWorkspaceTreeNode> Children { get; }
        void AddChildNode(IWorkspaceTreeNode childNode);

        bool IsSelected { get; set; }
        bool Filtered { get; set; }
        bool IsExpanded { get; set; }
    }
}
