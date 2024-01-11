using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.Editor.Shell.Document;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Workspace;
using Rubberduck.UI.Command;
using Rubberduck.UI.Command.Abstract;
using Rubberduck.UI.Command.SharedHandlers;
using Rubberduck.UI.Services;
using Rubberduck.UI.Services.Abstract;
using Rubberduck.UI.Shell.Document;
using Rubberduck.UI.Shell.StatusBar;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Rubberduck.Editor.Commands
{
    public class OpenDocumentCommand : CommandBase
    {
        private readonly IWorkspaceStateManager _workspaces;
        private readonly ShellProvider _shell;

        private readonly ShowRubberduckSettingsCommand _showSettingsCommand;
        private readonly CloseToolWindowCommand _closeToolWindowCommand;
        private readonly IDocumentStatusViewModel _activeDocumentStatus;

        private readonly Func<ILanguageClient?> _lsp;

        public OpenDocumentCommand(UIServiceHelper service,
            IWorkspaceStateManager workspaces,
            ShellProvider shell,
            ShowRubberduckSettingsCommand showSettingsCommand,
            CloseToolWindowCommand closeToolWindow,
            IDocumentStatusViewModel activeDocumentStatus, Func<ILanguageClient?> lsp)
            : base(service)
        {
            _workspaces = workspaces;
            _shell = shell;
            _closeToolWindowCommand = closeToolWindow;
            _showSettingsCommand = showSettingsCommand;
            _activeDocumentStatus = activeDocumentStatus;
            AddToCanExecuteEvaluation(e => true);

            _lsp = lsp;
        }

        protected async override Task OnExecuteAsync(object? parameter)
        {
            if (parameter is WorkspaceFileUri uri)
            {
                var rootUri = _workspaces.ActiveWorkspace?.WorkspaceRoot;
                //var root = _workspaces.ActiveWorkspace?.WorkspaceRoot?.LocalPath ?? throw new InvalidOperationException();
                //var srcRoot = System.IO.Path.Combine(root, ProjectFile.SourceRoot);
                //var relativeUri = uri.OriginalString[1..][srcRoot.Length..];

                if (rootUri != null && (_workspaces.ActiveWorkspace?.TryGetWorkspaceFile(uri, out var file) ?? false)
                    && file != null && !file.IsMissing && !file.IsLoadError)
                {
                    UserControl view;
                    IDocumentTabViewModel document;
                    if (file.IsSourceFile)
                    {
                        document = new VBACodeDocumentTabViewModel(uri, file.Name, file.Content, false, _showSettingsCommand, _closeToolWindowCommand, _activeDocumentStatus);
                        view = new SourceCodeEditorControl() { DataContext = document };
                    }
                    else
                    {
                        switch (file.FileExtension)
                        {
                            case "md":
                                document = new MarkdownDocumentTabViewModel(uri, file.Name, file.Content, false, _showSettingsCommand, _closeToolWindowCommand, _activeDocumentStatus);
                                view = new MarkdownEditorControl() { DataContext = document };
                                break;
                            case "rdproj":
                                document = new RubberduckProjectDocumentTabViewModel(uri, ProjectFile.FileName /* TODO put the project name here */, file.Content, false, _showSettingsCommand, _closeToolWindowCommand, _activeDocumentStatus);
                                view = new SourceCodeEditorControl() { DataContext = document }; // TODO understand json as a different "language"
                                break;
                            default:
                                document = new TextDocumentTabViewModel(uri, file.Name, file.Content, false, _showSettingsCommand, _closeToolWindowCommand, _activeDocumentStatus);
                                view = new TextEditorControl() { DataContext = document };
                                break;
                        }
                    }

                    NotifyLanguageServer(file);
                    document.ContentControl = view;
                    _shell.ViewModel.DocumentWindows.Add(document);
                    _shell.ViewModel.ActiveDocumentTab = document;
                }
            }

            await Task.CompletedTask;
        }

        private void NotifyLanguageServer(WorkspaceFileInfo file)
        {
            var lsp = _lsp();
            if (lsp is null)
            {
                Service.LogWarning("LanguageServerClient is null; LSP server will not be notified.");
                return;
            }

            var absoluteUri = file.Uri.AbsoluteLocation;

            var languageId = file.IsSourceFile
                ? "vba"
                : "none";

            var textDocumentItem = new TextDocumentItem
            {
                Uri = absoluteUri,
                Version = 1,
                LanguageId = languageId,
                Text = file.OriginalContent
            };
            lsp.TextDocument.DidOpenTextDocument(new() { TextDocument = textDocumentItem });
            file.IsOpened = true;
        }
    }
}
