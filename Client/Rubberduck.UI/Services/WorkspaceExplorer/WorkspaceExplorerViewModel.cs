using Rubberduck.InternalApi.Model.Workspace;
using Rubberduck.SettingsProvider.Model.Editor.Tools;
using Rubberduck.UI.Command.SharedHandlers;
using Rubberduck.UI.Services.Abstract;
using Rubberduck.UI.WorkspaceExplorer;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;

namespace Rubberduck.UI.Services.WorkspaceExplorer
{

    public class WorkspaceExplorerViewModel : ToolWindowViewModelBase, IWorkspaceExplorerViewModel
    {
        private readonly IWorkspaceService _service;

        public WorkspaceExplorerViewModel(IWorkspaceService service, ShowRubberduckSettingsCommand showSettingsCommand, CloseToolWindowCommand closeToolwindowCommand)
            : base(DockingLocation.DockLeft, showSettingsCommand, closeToolwindowCommand)
        {
            _service = service;
            Workspaces = new(service.ProjectFiles.Select(workspace => WorkspaceViewModel.FromModel(workspace, _service)));

            _dispatcher = Dispatcher.CurrentDispatcher;

            service.WorkspaceOpened += OnWorkspaceOpened;
            service.WorkspaceClosed += OnWorkspaceClosed;
        }

        private Dispatcher _dispatcher;

        private void OnWorkspaceClosed(object? sender, WorkspaceServiceEventArgs e)
        {
            var item = Workspaces.SingleOrDefault(workspace => workspace.Uri == e.Uri);
            if (item != null)
            {
                _dispatcher.Invoke(() => Workspaces.Remove(item));
            }
        }

        private void OnWorkspaceOpened(object? sender, WorkspaceServiceEventArgs e)
        {
            var path = _service.FileSystem.Path.Combine(e.Uri.LocalPath, ProjectFile.FileName);
            var project = _service.ProjectFiles.SingleOrDefault(file => file.Uri.LocalPath == path);
            if (project != null)
            {
                _dispatcher.Invoke(() => Workspaces.Add(WorkspaceViewModel.FromModel(project, _service)));
            }
        }

        public override string Title { get; } = "Workspace Explorer"; // TODO localize
        public override string SettingKey { get; } = nameof(WorkspaceExplorerSettings);

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

        public void Load(ProjectFile workspace)
        {
            Workspaces.Add(WorkspaceViewModel.FromModel(workspace, _service));
        }
    }
}
