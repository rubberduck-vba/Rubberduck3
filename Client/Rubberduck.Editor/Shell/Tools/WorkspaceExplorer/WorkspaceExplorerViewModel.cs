using Rubberduck.Editor.Shell.Document.Tabs;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Workspace;
using Rubberduck.SettingsProvider.Model.Editor.Tools;
using Rubberduck.UI.Command;
using Rubberduck.UI.Command.Abstract;
using Rubberduck.UI.Command.SharedHandlers;
using Rubberduck.UI.Services;
using Rubberduck.UI.Services.Abstract;
using Rubberduck.UI.Shell;
using Rubberduck.UI.Shell.Document;
using Rubberduck.UI.WorkspaceExplorer;
using System;
using System.Collections.ObjectModel;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace Rubberduck.Editor.Shell.Tools.WorkspaceExplorer
{
    public class OpenDocumentCommand : CommandBase
    {
        private readonly IWorkspaceStateManager _workspaces;
        private readonly ShellProvider _shell;

        public OpenDocumentCommand(UIServiceHelper service, IWorkspaceStateManager workspaces, ShellProvider shell) : base(service)
        {
            _workspaces = workspaces;
            _shell = shell;
        }

        protected async override Task OnExecuteAsync(object? parameter)
        {
            if (parameter is Uri uri)
            {
                var root = _workspaces.ActiveWorkspace?.WorkspaceRoot?.LocalPath ?? throw new InvalidOperationException();
                var srcRoot = System.IO.Path.Combine(root, ProjectFile.SourceRoot);
                var relativeUri = uri.OriginalString[1..][srcRoot.Length..];

                if ((_workspaces.ActiveWorkspace?.TryGetWorkspaceFile(new Uri(relativeUri, UriKind.Relative), out var file) ?? false) 
                    && file != null && !file.IsMissing && !file.IsLoadError)
                {
                    object view;
                    IDocumentTabViewModel document;
                    if (file.IsSourceFile)
                    {
                        document = new VBACodeDocumentTabViewModel(uri, file.Name, file.Content);
                        view = new SourceCodeEditorControl() { DataContext = document };
                    }
                    else
                    {
                        switch (file.FileExtension)
                        {
                            case "md":
                                document = new MarkdownDocumentTabViewModel(uri, file.Name, file.Content);
                                view = new MarkdownEditorControl() { DataContext = document };
                                break;
                            case "rdproj":
                                document = new RubberduckProjectDocumentTabViewModel(uri, ProjectFile.FileName /* TODO put the project name here */, file.Content);
                                view = new SourceCodeEditorControl() { DataContext = document }; // TODO understand json as a different "language"
                                break;
                            default:
                                document = new TextDocumentTabViewModel(uri, file.Name, file.Content);
                                view = new TextEditorControl() { DataContext = document };
                                break;
                        }
                    }

                    _shell.ViewModel.Documents.Add(document);
                    _shell.View.AddDocument(view);
                    file.IsOpened = true;
                }
            }

            await Task.CompletedTask;
        }
    }

    public class WorkspaceExplorerViewModel : ToolWindowViewModelBase, IWorkspaceExplorerViewModel
    {
        private readonly IWorkspaceService _service;

        public WorkspaceExplorerViewModel(IWorkspaceService service, 
            ShowRubberduckSettingsCommand showSettingsCommand, 
            CloseToolWindowCommand closeToolwindowCommand,
            OpenDocumentCommand openDocumentCommand)
            : base(DockingLocation.DockLeft, showSettingsCommand, closeToolwindowCommand)
        {
            _service = service;
            Workspaces = new(service.ProjectFiles.Select(workspace => WorkspaceViewModel.FromModel(workspace, _service)));
            OpenDocumentCommand = openDocumentCommand;

            _dispatcher = Dispatcher.CurrentDispatcher;

            service.WorkspaceOpened += OnWorkspaceOpened;
            service.WorkspaceClosed += OnWorkspaceClosed;
        }

        public ICommand OpenDocumentCommand { get; }

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

        private IWorkspaceTreeNode? _selection;
        public IWorkspaceTreeNode? Selection
        {
            get => _selection;
            set
            {
                if (_selection != value)
                {
                    _selection = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<IWorkspaceViewModel> Workspaces { get; } = [];

        public void Load(ProjectFile workspace)
        {
            Workspaces.Add(WorkspaceViewModel.FromModel(workspace, _service));
        }
    }
}
