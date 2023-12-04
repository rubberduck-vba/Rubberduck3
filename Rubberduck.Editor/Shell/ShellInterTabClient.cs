using Dragablz;
using Rubberduck.UI.Shell;
using Rubberduck.UI.Shell.StatusBar;
using System.Windows;

namespace Rubberduck.Editor.Shell
{
    public class ShellInterTabClient : IInterTabClient
    {
        private readonly IShellStatusBarViewModel _status;

        public ShellInterTabClient(IShellStatusBarViewModel status)
        {
            _status = status;
        }

        public INewTabHost<Window> GetNewHost(IInterTabClient interTabClient, object partition, TabablzControl source)
        {
            var vm = new ChildWindowViewModel(interTabClient, _status, partition?.ToString() ?? "main");
            var view = new ChildWindow(vm);
            return new NewTabHost<Window>(view, view.Tabs);
        }

        public TabEmptiedResponse TabEmptiedHandler(TabablzControl tabControl, Window window) => TabEmptiedResponse.DoNothing;
    }
}
