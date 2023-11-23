using Dragablz;
using Rubberduck.InternalApi.Model.Abstract;
using Rubberduck.UI.Shell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Rubberduck.Editor.Shell
{
    public class ShellWindowViewModel : IShellWindowViewModel
    {
        private static readonly string _shellPartition = Guid.NewGuid().ToString();

        public ShellWindowViewModel(IInterTabClient interTabClient, StatusBarViewModel statusBar,
            FileCommandHandlers fileCommandHandlers)
        {
            InterTabClient = interTabClient;
            Partition = _shellPartition;

            StatusBar = statusBar;
            Documents = new ObservableCollection<IDocumentTabViewModel>();

            FileCommandHandlers = fileCommandHandlers;
        }

        public string Title => "Rubberduck Editor";

        public IEnumerable<IDocumentTabViewModel> Documents { get; init; }

        public object Partition { get; init; }
        public object InterTabClient { get; init; }

        public IStatusBarViewModel StatusBar { get; init; }

        public FileCommandHandlers FileCommandHandlers { get; init; }

        public void ClosingTabItemHandler(object sender, EventArgs e)
        {
            // var item = (ChildWindowViewModel)sender;
            // TODO notify language server of closed document URI
        }
    }
}
