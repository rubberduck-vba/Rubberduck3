using Dragablz;
using Rubberduck.Editor.Commands;
using Rubberduck.Editor.Shell.StatusBar;
using Rubberduck.UI;
using Rubberduck.UI.Chrome;
using Rubberduck.UI.Services;
using Rubberduck.UI.Shell;
using Rubberduck.UI.Shell.Document;
using Rubberduck.UI.Shell.StatusBar;
using Rubberduck.UI.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Rubberduck.Editor.Shell
{
    public class ShellWindowViewModel : ViewModelBase, IShellWindowViewModel
    {
        private static readonly string _documentsPartition = "documents";
        private static readonly string _toolwindowPartition = "toolwindows";
        private readonly UIServiceHelper _service;

        public ShellWindowViewModel(UIServiceHelper service, IInterTabClient interTabClient, IShellStatusBarViewModel statusBar,
            FileCommandHandlers fileCommandHandlers,
            ViewCommandHandlers viewCommandHandlers,
            ToolsCommandHandlers toolsCommandHandlers)
        {
            _service = service;
            DocumentsInterTabClient = interTabClient;

            StatusBar = statusBar;
            Documents = new ObservableCollection<IDocumentTabViewModel>();
            ToolWindows = new ObservableCollection<IToolWindowViewModel>();

            FileCommandHandlers = fileCommandHandlers;
            ViewCommandHandlers = viewCommandHandlers;
            ToolsCommandHandlers = toolsCommandHandlers;

            CommandBindings = fileCommandHandlers.CreateCommandBindings()
                .Concat(viewCommandHandlers.CreateCommandBindings())
                .Concat(toolsCommandHandlers.CreateCommandBindings());
        }

        public override IEnumerable<CommandBinding> CommandBindings { get; }
        public string Title => "Rubberduck Editor";

        public IEnumerable<IDocumentTabViewModel> Documents { get; init; }
        public IEnumerable<IToolWindowViewModel> ToolWindows { get; init; }

        public IShellStatusBarViewModel StatusBar { get; init; }

        public FileCommandHandlers FileCommandHandlers { get; init; }
        public ViewCommandHandlers ViewCommandHandlers { get; init; }
        public ToolsCommandHandlers ToolsCommandHandlers { get; set; }

        public IWindowChromeViewModel Chrome => throw new NotImplementedException();


        public IInterTabClient DocumentsInterTabClient { get; init; }
        public IInterTabClient ToolWindowInterTabClient { get; init; }

        public void ClosingTabItemHandler(object sender, EventArgs e)
        {
            // var item = (ChildWindowViewModel)sender;
            // TODO notify language server of closed document URI
        }
    }
}
