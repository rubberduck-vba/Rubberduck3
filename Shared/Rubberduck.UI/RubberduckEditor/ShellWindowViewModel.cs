using Dragablz;
using Rubberduck.UI.Xaml.Dependencies.Controls.StatusBar;
using Rubberduck.UI.Xaml.Shell;
using Rubberduck.UI.Xaml.Shell.Document;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Rubberduck.UI.RubberduckEditor
{
    public class ShellWindowViewModel : IWindowViewModel
    {
        private static readonly string _shellPartition = Guid.NewGuid().ToString();

        public ShellWindowViewModel(IInterTabClient interTabClient, IStatusBarViewModel statusBar, params IDocumentTabViewModel[] items)
        {
            InterTabClient = interTabClient;
            Partition = _shellPartition;

            StatusBar = statusBar;
            Items = new ObservableCollection<IDocumentTabViewModel>(items);
        }

        public string Title => "Rubberduck Editor";

        public IEnumerable<IDocumentTabViewModel> Items { get; init; }

        public object Partition { get; init; }
        public IInterTabClient InterTabClient { get; init; }

        public IStatusBarViewModel StatusBar { get; init; }

        public void ClosingTabItemHandler(object sender, EventArgs e)
        {
            var item = (ChildWindowViewModel)sender;
            // TODO notify language server of closed document URI
        }
    }
}
