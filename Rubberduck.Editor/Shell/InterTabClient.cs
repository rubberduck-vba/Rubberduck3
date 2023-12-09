using Dragablz;
using Rubberduck.UI.Shell;
using Rubberduck.UI.Windows;
using System.Windows;

namespace Rubberduck.Editor.Shell
{
    public abstract class InterTabClient : IInterTabClient
    {
        protected abstract string Partition { get; }
        protected abstract IDragablzWindowViewModel GetWindowViewModel(IInterTabClient interTabClient, string partition);
        protected abstract IShellChildWindow GetChildWindow(IDragablzWindowViewModel vm);

        public INewTabHost<Window> GetNewHost(IInterTabClient interTabClient, object partition, TabablzControl source)
        {
            var vm = GetWindowViewModel(interTabClient, partition?.ToString() ?? Partition);
            var view = GetChildWindow(vm);
            view.DataContext ??= vm;

            return new NewTabHost<Window>((Window)view, view.Tabs);
        }

        public virtual TabEmptiedResponse TabEmptiedHandler(TabablzControl tabControl, Window window) 
            => TabEmptiedResponse.CloseWindowOrLayoutBranch;
    }
}
