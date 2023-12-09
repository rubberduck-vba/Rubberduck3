using Dragablz;
using Rubberduck.UI.Shell.StatusBar;
using Rubberduck.UI.Windows;
using System;
using System.Collections.ObjectModel;

namespace Rubberduck.Editor.Shell
{
    public abstract class ChildWindowViewModel : IDragablzWindowViewModel
    {
        public ChildWindowViewModel(IInterTabClient interTabClient, string partition)
        {
            InterTabClient = interTabClient;
            Partition = partition;

            Title = "Rubberduck Editor"; // tab title?
        }

        public string Title { get; }

        public IInterTabClient InterTabClient { get; }
        public string Partition { get; }

        public ObservableCollection<object> Tabs { get; set; }

        public virtual void ClosingTabItemHandler(object sender, EventArgs e)
        {
            
        }
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
