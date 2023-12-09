using Dragablz;
using Rubberduck.UI.Shell;
using Rubberduck.UI.Shell.StatusBar;
using Rubberduck.UI.Windows;

namespace Rubberduck.Editor.Shell
{
    public class DocumentsInterTabClient : InterTabClient
    {
        private readonly IShellStatusBarViewModel _status;

        public DocumentsInterTabClient(IShellStatusBarViewModel status)
        {
            _status = status;
        }

        protected override string Partition => Partitions.Documents;

        protected override IShellChildWindow GetChildWindow(IDragablzWindowViewModel vm) 
            => new DocumentsShellChildWindow(vm);

        protected override IDragablzWindowViewModel GetWindowViewModel(IInterTabClient interTabClient, string partition) 
            => new DocumentShellWindowViewModel(interTabClient, _status);
    }
}
