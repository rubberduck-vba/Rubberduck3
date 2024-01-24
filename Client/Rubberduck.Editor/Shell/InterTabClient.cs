using Dragablz;
using Dragablz.Dockablz;
using Rubberduck.InternalApi.Settings.Model.Editor.Tools;
using Rubberduck.UI.Chrome;
using Rubberduck.UI.Shell;
using Rubberduck.UI.Shell.StatusBar;
using Rubberduck.UI.Windows;
using System.Linq;
using System.Windows;

namespace Rubberduck.Editor.Shell
{
    public class InterToolTabClient : IInterTabClient
    {
        private readonly IWindowChromeViewModel _chrome;

        public InterToolTabClient(IWindowChromeViewModel chrome) 
        {
            _chrome = chrome;
        }

        public INewTabHost<Window> GetNewHost(IInterTabClient interTabClient, object partition, TabablzControl source)
        {
            var vm = new ToolWindowShellWindowViewModel(interTabClient, _chrome);
            var view = new ShellChildToolWindow(vm);
            
            return new NewTabHost<Window>(view, view.Tabs);
        }

        public TabEmptiedResponse TabEmptiedHandler(TabablzControl tabControl, Window window)
        {
            if (window is ShellWindow)
            {
                if (tabControl.DataContext is IToolWindowViewModel tab)
                {
                    tab.DockingLocation = DockingLocation.None;
                }

            }
            return TabEmptiedResponse.DoNothing; // do not close the shell window from an intertab client!
        }
    }

    public class InterTabClient : IInterTabClient
    {
        private readonly IShellStatusBarViewModel _shellStatusBar;
        private readonly IWindowChromeViewModel _chrome;

        public InterTabClient(IShellStatusBarViewModel shellStatusBar, IWindowChromeViewModel chrome)
        {
            _shellStatusBar = shellStatusBar;
            _chrome = chrome;
        }

        public INewTabHost<Window> GetNewHost(IInterTabClient interTabClient, object partition, TabablzControl source)
        {
            var vm = new DocumentShellWindowViewModel(interTabClient, _shellStatusBar, _chrome);
            var view = new ShellChildWindow(vm);
            return new NewTabHost<Window>(view, view.Tabs);
        }

        public virtual TabEmptiedResponse TabEmptiedHandler(TabablzControl tabControl, Window window)
        {
            return TabEmptiedResponse.DoNothing; // do not close the shell window from an intertab client!
        }
    }
}
