using Dragablz;
using Rubberduck.UI.Abstract;
using System.Windows;

namespace Rubberduck.UI.Xaml
{
    public class InterTabClient : IInterTabClient
    {
        private readonly IStatusBarViewModel _status;

        public InterTabClient(IStatusBarViewModel status)
        {
            _status = status;
        }

        public INewTabHost<Window> GetNewHost(IInterTabClient interTabClient, object partition, TabablzControl source)
        {
            var vm = new ChildWindowViewModel(interTabClient, _status, partition);
            var view = new ChildWindow(vm);
            return new NewTabHost<ChildWindow>(view, view.Tabs);
        }

        public TabEmptiedResponse TabEmptiedHandler(TabablzControl tabControl, Window window) => TabEmptiedResponse.DoNothing;
    }

}
