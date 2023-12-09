using Dragablz;
using Rubberduck.Editor.Commands;
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
        private readonly UIServiceHelper _service;

        public ShellWindowViewModel(UIServiceHelper service, 
            IInterTabClient documentsInterTabClient, 
            IInterTabClient toolwindowInterTabClient, 
            IShellStatusBarViewModel statusBar,
            FileCommandHandlers fileCommandHandlers,
            ViewCommandHandlers viewCommandHandlers,
            ToolsCommandHandlers toolsCommandHandlers)
        {
            _service = service;
            DocumentsInterTabClient = documentsInterTabClient;
            ToolWindowInterTabClient = toolwindowInterTabClient;

            StatusBar = statusBar;
            Documents = [];
            ToolWindows = [];

            FileCommandHandlers = fileCommandHandlers;
            ViewCommandHandlers = viewCommandHandlers;
            ToolsCommandHandlers = toolsCommandHandlers;

            CommandBindings = fileCommandHandlers.CreateCommandBindings()
                .Concat(viewCommandHandlers.CreateCommandBindings())
                .Concat(toolsCommandHandlers.CreateCommandBindings());
        }

        public override IEnumerable<CommandBinding> CommandBindings { get; }
        public string Title => "Rubberduck Editor";

        public ObservableCollection<IDocumentTabViewModel> Documents { get; init; }
        public void AddDocument(IDocumentTabViewModel tab)
        {
            Documents.Add(tab);
        }

        public ObservableCollection<IToolWindowViewModel> ToolWindows { get; init; }
        public void AddToolWindow(IToolWindowViewModel tab)
        {
            ToolWindows.Add(tab);
        }

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
