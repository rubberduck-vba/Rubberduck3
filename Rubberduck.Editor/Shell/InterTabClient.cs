using Dragablz;
using Rubberduck.UI.Shell;
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
            => TabEmptiedResponse.CloseWindowOrLayoutBranch;
    }
}
