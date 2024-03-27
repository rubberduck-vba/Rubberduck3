using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.Editor.Shell.Document;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using Rubberduck.InternalApi.Services;
using Rubberduck.UI.Command;
using Rubberduck.UI.Command.Abstract;
using Rubberduck.UI.Command.SharedHandlers;
using Rubberduck.UI.Services;
using Rubberduck.UI.Shell.Document;
using Rubberduck.UI.Shell.StatusBar;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Rubberduck.Editor.Commands
{
    public class OpenDocumentCommand : CommandBase
    {
        private readonly IAppWorkspacesStateManager _workspaces;
        private readonly ShellProvider _shell;

        private readonly ShowRubberduckSettingsCommand _showSettingsCommand;
        private readonly CloseToolWindowCommand _closeToolWindowCommand;
        private readonly IDocumentStatusViewModel _activeDocumentStatus;

        private readonly Func<ILanguageClient> _lsp;
        private readonly Lazy<ILanguageClient> _languageClient;
        private ILanguageClient LanguageClient => _languageClient.Value;

        public OpenDocumentCommand(UIServiceHelper service,
            IAppWorkspacesStateManager workspaces,
            ShellProvider shell,
            ShowRubberduckSettingsCommand showSettingsCommand,
            CloseToolWindowCommand closeToolWindow,
            IDocumentStatusViewModel activeDocumentStatus, Func<ILanguageClient> lsp)
            : base(service)
        {
            _workspaces = workspaces;
            _shell = shell;
            _closeToolWindowCommand = closeToolWindow;
            _showSettingsCommand = showSettingsCommand;
            _activeDocumentStatus = activeDocumentStatus;
            AddToCanExecuteEvaluation(e => true);

            _lsp = lsp;
            _languageClient = new Lazy<ILanguageClient>(() => lsp.Invoke(), isThreadSafe: true);
        }

        protected async override Task OnExecuteAsync(object? parameter)
        {
            var workspace = _workspaces.ActiveWorkspace;
            if (LanguageClient != null && workspace != null && parameter is WorkspaceFileUri uri)
            {
                if (workspace.WorkspaceRoot != null && workspace.TryGetWorkspaceFile(uri, out var file) && file != null)
                {
                    //&& !file.IsMissing && !file.IsLoadError

                    UserControl view;
                    IDocumentTabViewModel document;

                    if (file is DocumentState state)
                    {
                        if (state is CodeDocumentState codeDocumentState)
                        {
                            if (codeDocumentState.Language.Id == SupportedLanguage.VBA.Id)
                            {
                                document = new VBACodeDocumentTabViewModel(codeDocumentState, isReadOnly: false, _showSettingsCommand, _closeToolWindowCommand, _activeDocumentStatus, _lsp, Service);
                                view = new SourceCodeEditorControl() { DataContext = document };
                            }
                            else if (codeDocumentState.Language.Id == SupportedLanguage.VB6.Id)
                            {
                                document = new VB6CodeDocumentTabViewModel(codeDocumentState, isReadOnly: false, _showSettingsCommand, _closeToolWindowCommand, _activeDocumentStatus, _lsp, Service);
                                view = new SourceCodeEditorControl() { DataContext = document };
                            }
                            else
                            {
                                throw new NotSupportedException();
                            }
                        }
                        else
                        {
                            switch (file.FileExtension)
                            {
                                case "md":
                                    document = new MarkdownDocumentTabViewModel(state, isReadOnly: false, _showSettingsCommand, _closeToolWindowCommand, _activeDocumentStatus);
                                    view = new MarkdownEditorControl() { DataContext = document };
                                    break;
                                case "rdproj":
                                    document = new RubberduckProjectDocumentTabViewModel(state, isReadOnly: false, _showSettingsCommand, _closeToolWindowCommand, _activeDocumentStatus);
                                    view = new SourceCodeEditorControl() { DataContext = document }; // TODO understand json as a different "language" / make a json language server
                                    break;
                                default:
                                    document = new TextDocumentTabViewModel(state, isReadOnly: false, _showSettingsCommand, _closeToolWindowCommand, _activeDocumentStatus);
                                    view = new TextEditorControl() { DataContext = document };
                                    break;
                            }
                        }

                        workspace.WorkspaceFileStateChanged += (sender, args) =>
                        {
                            if (args.Uri == state.Uri && workspace.TryGetWorkspaceFile(args.Uri, out var updated) && updated != null)
                            {
                                document.DocumentState = updated;
                            }
                        };
                    }
                    else
                    {
                        throw new InvalidOperationException("document state was unexpectedly null.");
                    }

                    file = file with { IsOpened = true };
                    NotifyLanguageServer(file);

                    document.ContentControl = view;
                    _shell.ViewModel.DocumentWindows.Add(document);
                    _shell.ViewModel.ActiveDocumentTab = document;
                }
                else
                {
                    throw new FileNotFoundException($"File '{uri}' is present in the workspace folder, but not included in this workspace. Include it in the project?");
                }
            }

            await Task.CompletedTask;
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
