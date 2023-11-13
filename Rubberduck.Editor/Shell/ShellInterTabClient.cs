using Dragablz;
using Rubberduck.InternalApi.Model.Abstract;
using System.Windows;

namespace Rubberduck.Editor.Shell
{
    public class ShellInterTabClient : IInterTabClient
    {
        private readonly StatusBarViewModel _status;

        public ShellInterTabClient(StatusBarViewModel status)
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
