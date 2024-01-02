using Dragablz;
using Rubberduck.SettingsProvider.Model.Editor.Tools;
using Rubberduck.UI;
using Rubberduck.UI.Chrome;
using Rubberduck.UI.Shell.Document;
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
        public ChildWindowViewModel(IInterTabClient interTabClient, string partition, IWindowChromeViewModel chrome)
        {
            InterTabClient = interTabClient;
            Partition = partition;
            Chrome = chrome;

            Tabs = [];
        }

        public string Title { get; } = "Rubberduck Editor";
        public ObservableCollection<IDocumentTabViewModel> Documents { get; init; }

        public IWindowChromeViewModel Chrome { get; }
        public IInterTabClient InterTabClient { get; }
        public string Partition { get; }

        public ObservableCollection<object> Tabs { get; set; }

        public virtual TabEmptiedResponse OnTabControlEmptied(TabablzControl tabControl, Window window)
            => TabEmptiedResponse.CloseWindowOrLayoutBranch;
    }

    public class DocumentShellWindowViewModel : ChildWindowViewModel
    {
        public DocumentShellWindowViewModel(IInterTabClient interTabClient, IShellStatusBarViewModel statusBar, IWindowChromeViewModel chrome) 
            : base(interTabClient, Partitions.Documents, chrome)
        {
            StatusBar = statusBar;
        }


        public IShellStatusBarViewModel StatusBar { get; }
    }

    public class ToolWindowShellWindowViewModel : ChildWindowViewModel
    {
        public ToolWindowShellWindowViewModel(IInterTabClient interTabClient, IWindowChromeViewModel chrome) 
            : base(interTabClient, Partitions.Toolwindows, chrome)
        {
        }

        public DockingLocation DockingLocation { get; set; } = DockingLocation.None;
    }
}
