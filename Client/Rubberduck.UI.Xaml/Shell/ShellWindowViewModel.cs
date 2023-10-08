using Dragablz;
using Rubberduck.UI.Abstract;
using Rubberduck.Unmanaged.Abstract.SafeComWrappers;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Documents;

namespace Rubberduck.UI.Xaml.Shell
{
    public class ShellWindowViewModel : IWindowViewModel
    {
        public ShellWindowViewModel(IInterTabClient interTabClient, IStatusBarViewModel status)
        {
            Status = status;
            InterTabClient = interTabClient;
            Partition = "1";

            Items = new ObservableCollection<DocumentTabViewModel>(new DocumentTabViewModel[]
            {
                new MarkdownDocumentTabViewModel(new Uri("file://rd3/welcome.md"), "Welcome", "TODO"),
                new CodeDocumentTabViewModel(new Uri("file://workspace/Module1.bas"), "Module1", "Option Explicit\r\n"),
            });
        }

        public string Title => "Rubberduck Editor";

        public ObservableCollection<DocumentTabViewModel> Items { get; init; }

        public object Partition { get; init; }
        public IInterTabClient InterTabClient { get; init; }

        public IStatusBarViewModel Status { get; init; }

        public void ClosingTabItemHandler(object sender, EventArgs e)
        {
            var item = (ChildWindowViewModel)sender;
            // TODO notify language server of closed document URI
        }
    }
}
