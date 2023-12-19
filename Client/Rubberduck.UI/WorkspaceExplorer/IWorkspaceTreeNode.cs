using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Rubberduck.UI.WorkspaceExplorer
{
    public interface IWorkspaceTreeNode : INotifyPropertyChanged
    {
        Uri Uri { get; }
        string FileName { get; }
        string Name { get; }
        ObservableCollection<IWorkspaceTreeNode> Children { get; }
        void AddChildNode(IWorkspaceTreeNode childNode);

        bool IsSelected { get; set; }
        bool Filtered { get; set; }
        bool IsExpanded { get; set; }
    }
}
