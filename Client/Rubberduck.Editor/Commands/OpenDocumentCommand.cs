using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.Editor.Shell.Document;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Workspace;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using Rubberduck.InternalApi.Services;
using Rubberduck.UI.Command;
using Rubberduck.UI.Command.Abstract;
using Rubberduck.UI.Command.SharedHandlers;
using Rubberduck.UI.Services;
using Rubberduck.UI.Shell.Document;
using Rubberduck.UI.Shell.StatusBar;
using System;
using System.Threading;
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
            var workspace = _workspaces.ActiveWorkspace;
            if (workspace != null && parameter is WorkspaceFileUri uri)
            {
                var rootUri = workspace.WorkspaceRoot;
                //var root = _workspaces.ActiveWorkspace?.WorkspaceRoot?.LocalPath ?? throw new InvalidOperationException();
                //var srcRoot = System.IO.Path.Combine(root, ProjectFile.SourceRoot);
                //var relativeUri = uri.OriginalString[1..][srcRoot.Length..];

                if (rootUri != null && workspace.TryGetWorkspaceFile(uri, out var file) && file != null)
                {
                    //&& file != null && !file.IsMissing && !file.IsLoadError

                    UserControl view;
                    IDocumentTabViewModel document;
                    
                    if (file is DocumentState)
                    {
                        await RequestFoldingsAsync(file, TimeSpan.Zero); // TODO configure timeout

                        document = new VBACodeDocumentTabViewModel(uri, file.Name, file.Text, isReadOnly: false, _showSettingsCommand, _closeToolWindowCommand, _activeDocumentStatus);
                        view = new SourceCodeEditorControl() { DataContext = document };
                    }
                    else
                    {
                        switch (file.FileExtension)
                        {
                            case "md":
                                document = new MarkdownDocumentTabViewModel(uri, file.Name, file.Text, isReadOnly: false, _showSettingsCommand, _closeToolWindowCommand, _activeDocumentStatus);
                                view = new MarkdownEditorControl() { DataContext = document };
                                break;
                            case "rdproj":
                                document = new RubberduckProjectDocumentTabViewModel(uri, workspace.ProjectName, file.Text, isReadOnly: false, _showSettingsCommand, _closeToolWindowCommand, _activeDocumentStatus);
                                view = new SourceCodeEditorControl() { DataContext = document }; // TODO understand json as a different "language"
                                break;
                            default:
                                document = new TextDocumentTabViewModel(uri, file.Name, file.Text, isReadOnly: false, _showSettingsCommand, _closeToolWindowCommand, _activeDocumentStatus);
                                view = new TextEditorControl() { DataContext = document };
                                break;
                        }
                    }

                    file = file.WithOpened(true);
                    NotifyLanguageServer(file);

                    document.ContentControl = view;
                    _shell.ViewModel.DocumentWindows.Add(document);
                    _shell.ViewModel.ActiveDocumentTab = document;
                }
            }

            await Task.CompletedTask;
        }

        private async Task<Container<FoldingRange>> RequestFoldingsAsync(DocumentState file, TimeSpan timeout)
        {
            var lsp = _lsp();
            if (lsp is null)
            {
                throw new InvalidOperationException("LanguageServerClient is unexpectedly null.");
            }

            Service.LogInformation($"Requesting folding ranges for document '{file.Name}'...");

            var request = new FoldingRangeRequestParam { TextDocument = file.Id };
            var tokenSource = new CancellationTokenSource(timeout);

            var response = await lsp.RequestFoldingRange(request, CancellationToken.None);
            if (response is Container<FoldingRange> foldings)
            {
                Service.LogInformation($" Received folding ranges for document '{file.Name}'.");
                return foldings;
            }

            return new Container<FoldingRange>();
        }

        private void NotifyLanguageServer(DocumentState file)
        {
            var lsp = _lsp();
            if (lsp is null)
            {
                Service.LogWarning("LanguageServerClient is null; LSP server will not be notified.");
                return;
            }

            var absoluteUri = file.Uri.AbsoluteLocation;
            
            var textDocumentItem = new TextDocumentItem
            {
                Uri = absoluteUri,
                Version = 1,
                LanguageId = SupportedLanguage.VBA.Id,
                Text = file.Text,
            };
            lsp.TextDocument.DidOpenTextDocument(new() { TextDocument = textDocumentItem });
        }
    }
}
