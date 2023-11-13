using Dragablz;
using Rubberduck.InternalApi.Model.Abstract;
using Rubberduck.UI.NewProject;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Rubberduck.Editor.Shell
{
    public class ShellWindowViewModel : IWindowViewModel
    {
        private static readonly string _shellPartition = Guid.NewGuid().ToString();

        public ShellWindowViewModel(IInterTabClient interTabClient, StatusBarViewModel statusBar/*, params IDocumentTabViewModel[] items*/,
            NewProjectCommand newProjectCommand)
        {
            InterTabClient = interTabClient;
            Partition = _shellPartition;

            StatusBar = statusBar;
            Items = new ObservableCollection<IDocumentTabViewModel>();

            NewProjectCommand = newProjectCommand;
        }

        public string Title => "Rubberduck Editor";

        public IEnumerable<IDocumentTabViewModel> Items { get; init; }

        public object Partition { get; init; }
        public object InterTabClient { get; init; }

        public NewProjectCommand NewProjectCommand { get; init; }
        public StatusBarViewModel StatusBar { get; init; }

        public void ClosingTabItemHandler(object sender, EventArgs e)
        {
            // var item = (ChildWindowViewModel)sender;
            // TODO notify language server of closed document URI
        }
    }
}
