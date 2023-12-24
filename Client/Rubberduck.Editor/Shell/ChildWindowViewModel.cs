using Dragablz;
using Rubberduck.UI;
using Rubberduck.UI.Chrome;
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

            Tabs = [];
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
        public DocumentShellWindowViewModel(IInterTabClient interTabClient, IShellStatusBarViewModel statusBar, IWindowChromeViewModel chrome) 
            : base(interTabClient, Partitions.Documents)
        {
            Chrome = chrome;
            StatusBar = statusBar;
        }

        public IWindowChromeViewModel Chrome { get; }

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
