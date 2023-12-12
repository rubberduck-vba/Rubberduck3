using Dragablz;
using Dragablz.Dockablz;
using Rubberduck.SettingsProvider.Model.Editor.Tools;
using Rubberduck.UI.Shell;
using Rubberduck.UI.Windows;
using System.Linq;
using System.Windows;

namespace Rubberduck.Editor.Shell
{
    public class InterTabClient : IInterTabClient
    {
        public INewTabHost<Window> GetNewHost(IInterTabClient interTabClient, object partition, TabablzControl source)
        {
            var vm = new ChildWindowViewModel(interTabClient, partition?.ToString() ?? "root");
            var view = new ShellChildWindow(vm);
            return new NewTabHost<Window>(view, view.Tabs);
        }

        public virtual TabEmptiedResponse TabEmptiedHandler(TabablzControl tabControl, Window window)
        {
            if (window is ShellWindow shell)
            {
                var vm = (IShellWindowViewModel)shell.DataContext;
                if (tabControl.Name == "LeftPaneToolTabs")
                {
                    // FIXME this should not be necessary, something is off with the bindings.
                    foreach (var toolwindow in vm.ToolWindows.Where(e => e.DockingLocation == DockingLocation.DockLeft).ToList())
                    {
                        vm.ToolWindows.Remove(toolwindow);
                    }
                }
                shell.LeftToolPanel.Width = double.NaN;
                return TabEmptiedResponse.DoNothing; // do not close the shell window from an intertab client!
            }
            return window is ShellWindow ? TabEmptiedResponse.DoNothing : TabEmptiedResponse.CloseWindowOrLayoutBranch;
        }
    }
}
