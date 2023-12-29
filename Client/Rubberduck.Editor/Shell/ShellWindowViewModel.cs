using Dragablz;
using Rubberduck.Editor.Commands;
using Rubberduck.SettingsProvider.Model.Editor.Tools;
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
            InterTabClient interTabClient, 
            InterToolTabClient interToolTabClient,
            IShellStatusBarViewModel statusBar,
            IWindowChromeViewModel chrome,
            FileCommandHandlers fileCommandHandlers,
            ViewCommandHandlers viewCommandHandlers,
            ToolsCommandHandlers toolsCommandHandlers)
        {
            _service = service;
            InterTabClient = interTabClient;
            InterToolTabClient = interToolTabClient;

            StatusBar = statusBar;
            Chrome = chrome;

            FileCommandHandlers = fileCommandHandlers;
            ViewCommandHandlers = viewCommandHandlers;
            ToolsCommandHandlers = toolsCommandHandlers;

            LeftToolPanel = new ToolPanelViewModel(DockingLocation.DockLeft);
            RightToolPanel = new ToolPanelViewModel(DockingLocation.DockRight);
            BottomToolPanel = new ToolPanelViewModel(DockingLocation.DockBottom);

            CommandBindings = fileCommandHandlers.CreateCommandBindings()
                .Concat(viewCommandHandlers.CreateCommandBindings())
                .Concat(toolsCommandHandlers.CreateCommandBindings());
        }

        public override IEnumerable<CommandBinding> CommandBindings { get; }
        public string Title => "Rubberduck Editor";

        public ObservableCollection<IDocumentTabViewModel> DocumentWindows { get; init; } = [];
        public ObservableCollection<IToolWindowViewModel> LeftPanelToolWindows { get; init; } = [];
        public ObservableCollection<IToolWindowViewModel> RightPanelToolWindows { get; init; } = [];
        public ObservableCollection<IToolWindowViewModel> BottomPanelToolWindows { get; init; } = [];

        public int FixedDocumentTabs => DocumentWindows.Count(e => e.IsPinned);
        public int FixedLeftToolTabs => LeftPanelToolWindows.Count(e => e.IsPinned);
        public int FixedRightToolTabs => RightPanelToolWindows.Count(e => e.IsPinned);
        public int FixedBottomToolTabs => BottomPanelToolWindows.Count(e => e.IsPinned);

        public IShellStatusBarViewModel StatusBar { get; init; }

        public FileCommandHandlers FileCommandHandlers { get; init; }
        public ViewCommandHandlers ViewCommandHandlers { get; init; }
        public ToolsCommandHandlers ToolsCommandHandlers { get; set; }

        public IWindowChromeViewModel Chrome { get; }

        public IInterTabClient InterTabClient { get; init; }
        public IInterTabClient InterToolTabClient { get; init; }

        public ItemActionCallback ClosingTabItemHandler => OnTabClosed;

        public IToolPanelViewModel LeftToolPanel { get; }

        public IToolPanelViewModel RightToolPanel { get; }

        public IToolPanelViewModel BottomToolPanel { get; }

        private void OnTabClosed(ItemActionCallbackArgs<TabablzControl> args)
        {
            /* TODO prompt to save changes, offer to cancel, etc.*/
            //var vm = args.DragablzItem.DataContext as ITabViewModel;
            //args.Cancel();
        }
    }

    /// <summary>
    /// The view model for the toolpanel expander sections.
    /// </summary>
    public class ToolPanelViewModel : ViewModelBase, IToolPanelViewModel
    {
        public ToolPanelViewModel(DockingLocation location)
        {
            PanelLocation = location;
        }

        public DockingLocation PanelLocation { get; init; }
        public bool IsDocked => PanelLocation != DockingLocation.None;

        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isPinned;
        public bool IsPinned
        {
            get => _isPinned;
            set
            {
                if (_isPinned != value)
                {
                    _isPinned = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
