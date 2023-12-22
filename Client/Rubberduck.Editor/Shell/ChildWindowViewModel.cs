using Dragablz;
using Rubberduck.UI;
using Rubberduck.UI.Shell.StatusBar;
using Rubberduck.UI.Windows;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace Rubberduck.Editor.Shell
{
    /// <summary>
    /// A model for a document or toolwindow tab that spawned a new window, which is now a new tab host.
    /// </summary>
    public class ChildWindowViewModel : ViewModelBase, IDragablzWindowViewModel
    {
        public ChildWindowViewModel(IInterTabClient interTabClient, string partition)
        {
            InterTabClient = interTabClient;
            Partition = partition;
        }

        public string Title { get; } = "Rubberduck Editor";

        public IInterTabClient InterTabClient { get; }
        public string Partition { get; }

        public ObservableCollection<object> Tabs { get; set; }

        public virtual TabEmptiedResponse OnTabControlEmptied(TabablzControl tabControl, Window window)
            => TabEmptiedResponse.CloseWindowOrLayoutBranch;
    }

    public class DocumentShellWindowViewModel : ChildWindowViewModel
    {
        public DocumentShellWindowViewModel(IInterTabClient interTabClient, IShellStatusBarViewModel statusBar) 
            : base(interTabClient, Partitions.Documents)
        {
            StatusBar = statusBar;
        }

        public IShellStatusBarViewModel StatusBar { get; }
    }

    public class ToolWindowShellWindowViewModel : ChildWindowViewModel
    {
        public ToolWindowShellWindowViewModel(IInterTabClient interTabClient) 
            : base(interTabClient, Partitions.Toolwindows)
        {
        }
    }
}
