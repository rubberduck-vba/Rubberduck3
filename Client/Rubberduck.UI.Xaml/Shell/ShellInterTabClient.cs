using Dragablz;
using Rubberduck.UI.Xaml.Dependencies.Controls.StatusBar;
using System.Windows;

namespace Rubberduck.UI.Xaml.Shell
{
    public class ShellInterTabClient : IInterTabClient
    {
        private readonly IStatusBarViewModel _status;

        public ShellInterTabClient(IStatusBarViewModel status)
        {
            _status = status;
        }

        public INewTabHost<Window> GetNewHost(IInterTabClient interTabClient, object partition, TabablzControl source)
        {
            var vm = new ChildWindowViewModel(interTabClient, _status, partition);
            var view = new ChildWindow(vm);
            return new NewTabHost<Window>(view, view.Tabs);
        }

        public TabEmptiedResponse TabEmptiedHandler(TabablzControl tabControl, Window window) => TabEmptiedResponse.DoNothing;
    }
}
