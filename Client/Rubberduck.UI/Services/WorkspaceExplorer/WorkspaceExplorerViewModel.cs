using Rubberduck.InternalApi.Model.Workspace;
using Rubberduck.SettingsProvider.Model.Editor.Tools;
using Rubberduck.UI.Command.SharedHandlers;
using Rubberduck.UI.Services.Abstract;
using Rubberduck.UI.WorkspaceExplorer;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Rubberduck.UI.Services.WorkspaceExplorer
{
    public class WorkspaceExplorerViewModel : ViewModelBase, IWorkspaceExplorerViewModel
    {
        private readonly IWorkspaceService _service;

        public WorkspaceExplorerViewModel()
        {
            /* DESIGNER */
        }

        public WorkspaceExplorerViewModel(IWorkspaceService service, ShowRubberduckSettingsCommand showSettingsCommand, CloseToolWindowCommand closeToolwindowCommand)
        {
            _service = service;
            ShowSettingsCommand = showSettingsCommand;
            CloseToolWindowCommand = closeToolwindowCommand;
            Workspaces = new(service.ProjectFiles.Select(workspace => WorkspaceViewModel.FromModel(workspace, _service)));

            service.WorkspaceOpened += OnWorkspaceOpened;
            service.WorkspaceClosed += OnWorkspaceClosed;
        }

        private void OnWorkspaceClosed(object? sender, WorkspaceServiceEventArgs e)
        {
            var item = Workspaces.SingleOrDefault(workspace => workspace.Uri == e.Uri);
            if (item != null)
            {
                Workspaces.Remove(item);
            }
        }

        private void OnWorkspaceOpened(object? sender, WorkspaceServiceEventArgs e)
        {
            var project = _service.ProjectFiles.SingleOrDefault(file => file.Uri == e.Uri);
            if (project != null)
            {
                Workspaces.Add(WorkspaceViewModel.FromModel(project, _service));
            }
        }

        public string Title { get; set; } = "Workspace Explorer"; // TODO localize
        public string AcceptButtonText { get; set; }
        public string CancelButtonText { get; set; }
        public ICommand ShowSettingsCommand { get; }
        public ICommand CloseToolWindowCommand { get; }
        public string ShowSettingsCommandParameter => SettingKey;
        public bool ShowPinButton { get; } = true;
        public bool ShowGearButton { get; } = true;
        public bool ShowCloseButton { get; } = true;

        public string SettingKey { get; } = nameof(WorkspaceExplorerSettings);

        private WorkspaceTreeNodeViewModel? _selection;
        public WorkspaceTreeNodeViewModel? Selection
        {
            get => _selection;
            set
            {
                if (_selection != value)
                {
                    _selection = value;
                    OnPropertyChanged();
                    SelectionInfo = _selection;
                }
            }
        }

        private IWorkspaceUriInfo? _selectionInfo;

        public IWorkspaceUriInfo? SelectionInfo
        {
            get => _selectionInfo;
            private set
            {
                if (_selectionInfo != value)
                {
                    _selectionInfo = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(HasSelectionInfo));
                }
            }
        }

        public bool HasSelectionInfo => _selectionInfo != null;

        public ObservableCollection<WorkspaceViewModel> Workspaces { get; } = new();

        public DockingLocation DockingLocation { get; set; } = DockingLocation.DockLeft;
        public object Header
        {
            get => Title;
            set
            {
                if (Title != value?.ToString())
                {
                    Title = value?.ToString() ?? string.Empty;
                    OnPropertyChanged();
                }
            }
        }

        private object _control;
        public object Content
        {
            get => _control;
            set
            {
                if (_control != value) 
                {
                    _control = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isSelected = true;
        public bool IsSelected 
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
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

        public void Load(ProjectFile workspace)
        {
            Workspaces.Add(WorkspaceViewModel.FromModel(workspace, _service));
        }
    }
}
