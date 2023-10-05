using Dragablz;
using Rubberduck.UI.Abstract;

namespace Rubberduck.UI.Xaml.Shell
{
    public class ChildWindowViewModel : ShellWindowViewModel
    {
        public ChildWindowViewModel(IInterTabClient interTabClient, IStatusBarViewModel status, object partition)
            : base(interTabClient, status)
        {
            Partition = partition;
        }
    }
}
