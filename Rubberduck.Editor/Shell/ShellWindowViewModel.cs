using Dragablz;
using Rubberduck.Editor.Commands;
using Rubberduck.Editor.Shell.StatusBar;
using Rubberduck.UI;
using Rubberduck.UI.Chrome;
using Rubberduck.UI.Services;
using Rubberduck.UI.Shell;
using Rubberduck.UI.Shell.Document;
using Rubberduck.UI.Shell.StatusBar;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Rubberduck.Editor.Shell
{
    public class ShellWindowViewModel : ViewModelBase, IShellWindowViewModel
    {
        private static readonly string _shellPartition = Guid.NewGuid().ToString();
        private readonly UIServiceHelper _service;

        public ShellWindowViewModel(UIServiceHelper service, IInterTabClient interTabClient, StatusBarViewModel statusBar,
            FileCommandHandlers fileCommandHandlers,
            ToolsCommandHandlers toolsCommandHandlers)
        {
            _service = service;
            InterTabClient = interTabClient;
            Partition = _shellPartition;

            StatusBar = statusBar;
            Documents = new ObservableCollection<IDocumentTabViewModel>();

            FileCommandHandlers = fileCommandHandlers;
            ToolsCommandHandlers = toolsCommandHandlers;

            CommandBindings = fileCommandHandlers.CreateCommandBindings()
                .Concat(toolsCommandHandlers.CreateCommandBindings()).ToList();
        }

        public override IEnumerable<CommandBinding> CommandBindings { get; }
        public string Title => "Rubberduck Editor";

        public IEnumerable<IDocumentTabViewModel> Documents { get; init; }

        public object Partition { get; init; }
        public object InterTabClient { get; init; }

        public IStatusBarViewModel StatusBar { get; init; }

        public FileCommandHandlers FileCommandHandlers { get; init; }

        public ToolsCommandHandlers ToolsCommandHandlers { get; set; }

        public IWindowChromeViewModel Chrome => throw new NotImplementedException();


        public void ClosingTabItemHandler(object sender, EventArgs e)
        {
            // var item = (ChildWindowViewModel)sender;
            // TODO notify language server of closed document URI
        }
    }
}
