using Dragablz;
using Rubberduck.Editor.Commands;
using Rubberduck.InternalApi.Settings.Model.Editor.Tools;
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
using System.Collections.Specialized;
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

            DocumentWindows.CollectionChanged += OnWindowCollectionChanged;
            LeftPanelToolWindows.CollectionChanged += OnWindowCollectionChanged;
            RightPanelToolWindows.CollectionChanged += OnWindowCollectionChanged;
            BottomPanelToolWindows.CollectionChanged += OnWindowCollectionChanged;
        }

        private void OnWindowCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (e.NewItems?[0] is IToolWindowViewModel newItem)
                {
                    if (sender == DocumentWindows)
                    {
                        newItem.DockingLocation = DockingLocation.DocumentPane;
                    }
                    else if (sender == LeftPanelToolWindows)
                    {
                        newItem.DockingLocation = DockingLocation.DockLeft;
                    }
                    else if (sender == RightPanelToolWindows)
                    {
                        newItem.DockingLocation = DockingLocation.DockRight;
                    }
                    else if (sender == BottomPanelToolWindows)
                    {
                        newItem.DockingLocation = DockingLocation.DockBottom;
                    }
                    else
                    {
                        newItem.DockingLocation = DockingLocation.None;
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (e.OldItems?[0] is IToolWindowViewModel deletedItem)
                {
                    switch (deletedItem.DockingLocation)
                    {
                        case DockingLocation.None:
                            break;
                        case DockingLocation.DocumentPane:
                            break;
                        case DockingLocation.DockLeft:
                            break;
                        case DockingLocation.DockRight:
                            break;
                        case DockingLocation.DockBottom:
                            break;
                    }
                }
            }
        }

        public override IEnumerable<CommandBinding> CommandBindings { get; }
        public string Title => "Rubberduck Editor";

        public ObservableCollection<IDocumentTabViewModel> DocumentWindows { get; init; } = [];
        public ObservableCollection<IToolWindowViewModel> FloatingToolwindows { get; init; } = [];
        public ObservableCollection<IToolWindowViewModel> LeftPanelToolWindows { get; init; } = [];
        public ObservableCollection<IToolWindowViewModel> RightPanelToolWindows { get; init; } = [];
        public ObservableCollection<IToolWindowViewModel> BottomPanelToolWindows { get; init; } = [];

        private IDocumentTabViewModel _activeDocumentTab;
        public IDocumentTabViewModel ActiveDocumentTab 
        {
            get => _activeDocumentTab;
            set
            {
                if (_activeDocumentTab != value)
                {
                    _activeDocumentTab = value;
                    OnPropertyChanged();

                    if (value != null)
                    {
                        foreach (var tab in DocumentWindows)
                        {
                            tab.IsSelected = false;
                        }
                        _activeDocumentTab.IsSelected = true;
                        StatusBar.ActiveDocumentStatus.DocumentType = _activeDocumentTab.DocumentType;
                        StatusBar.ActiveDocumentStatus.DocumentName = _activeDocumentTab.Title;
                        StatusBar.ActiveDocumentStatus.IsReadOnly = _activeDocumentTab.IsReadOnly;
                    }
                }
            }
        }
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
            var vm = args.DragablzItem.DataContext as ITabViewModel;
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
