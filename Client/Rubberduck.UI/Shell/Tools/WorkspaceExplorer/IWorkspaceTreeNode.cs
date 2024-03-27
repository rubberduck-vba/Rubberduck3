using Rubberduck.InternalApi.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Rubberduck.UI.Shell.Tools.WorkspaceExplorer
{
    public interface IWorkspaceFolderViewModel : IWorkspaceTreeNode
    {

    }

    public interface IWorkspaceTreeNode : INotifyPropertyChanged
    {
        WorkspaceUri Uri { get; }
        string FileName { get; }
        string Name { get; }
        ObservableCollection<IWorkspaceTreeNode> Children { get; }
        void AddChildNode(IWorkspaceTreeNode childNode);

        bool IsSelected { get; set; }
        bool Filtered { get; set; }
        bool IsExpanded { get; set; }
    }
}
