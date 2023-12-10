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
            IInterTabClient interTabClient, 
            IShellStatusBarViewModel statusBar,
            FileCommandHandlers fileCommandHandlers,
            ViewCommandHandlers viewCommandHandlers,
            ToolsCommandHandlers toolsCommandHandlers)
        {
            _service = service;
            InterTabClient = interTabClient;

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
        public ObservableCollection<IToolWindowViewModel> ToolWindows { get; init; }

        public IShellStatusBarViewModel StatusBar { get; init; }

        public FileCommandHandlers FileCommandHandlers { get; init; }
        public ViewCommandHandlers ViewCommandHandlers { get; init; }
        public ToolsCommandHandlers ToolsCommandHandlers { get; set; }

        public IWindowChromeViewModel Chrome => throw new NotImplementedException();

        public IInterTabClient InterTabClient { get; init; }

        public ItemActionCallback ClosingTabItemHandler => OnTabClosed;

        private void OnTabClosed(ItemActionCallbackArgs<TabablzControl> args)
        {
            /* TODO prompt to save changes, offer to cancel, etc.*/
            //var vm = args.DragablzItem.DataContext as ITabViewModel;
            //args.Cancel();
        }
    }
}
