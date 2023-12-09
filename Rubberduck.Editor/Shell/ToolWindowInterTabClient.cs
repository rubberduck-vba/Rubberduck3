using Dragablz;
using Rubberduck.UI.Shell;
using Rubberduck.UI.Windows;


namespace Rubberduck.Editor.Shell
{
    public class ToolWindowInterTabClient : InterTabClient
    {
        protected override string Partition => Partitions.Toolwindows;

        protected override IShellChildWindow GetChildWindow(IDragablzWindowViewModel vm)
            => new ToolwindowsShellChildWindow(vm);

        protected override IDragablzWindowViewModel GetWindowViewModel(IInterTabClient interTabClient, string partition)
            => new ToolWindowShellWindowViewModel(interTabClient);
    }
}
