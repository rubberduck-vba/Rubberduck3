using Dragablz;
using Rubberduck.UI.Shell.StatusBar;
using Rubberduck.UI.Windows;
using System;

namespace Rubberduck.Editor.Shell
{
    public class ChildWindowViewModel : IDragablzWindowViewModel
    {
        public ChildWindowViewModel(IInterTabClient interTabClient, IShellStatusBarViewModel statusBar, string partition)
        {
            InterTabClient = interTabClient;
            Partition = partition;

            StatusBar = statusBar;
            Title = "Rubberduck Editor"; // tab title?
        }

        public string Title { get; }
        public IShellStatusBarViewModel StatusBar { get; }

        public IInterTabClient InterTabClient { get; }

        public string Partition { get; }

        public void ClosingTabItemHandler(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
