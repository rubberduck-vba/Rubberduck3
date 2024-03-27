using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using Rubberduck.InternalApi.Settings.Model.Editor;
using Rubberduck.UI.Command.SharedHandlers;
using Rubberduck.UI.Shell.Document;
using Rubberduck.UI.Shell.StatusBar;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using Rubberduck.InternalApi.Extensions;
using System.Linq;
using System.Threading;
using Rubberduck.UI.Services;

namespace Rubberduck.Editor.Shell.Document
{
    /// <summary>
    /// The base view model for a type of document tab that contains code managed by a language server.
    /// </summary>
    public abstract class CodeDocumentTabViewModel : DocumentTabViewModel, ICodeDocumentTabViewModel
    {
        private readonly Lazy<ILanguageClient> _languageClient;
        private readonly UIServiceHelper _service;

        public CodeDocumentTabViewModel(CodeDocumentState state, bool isReadOnly,
            ShowRubberduckSettingsCommand showSettingsCommand,
            CloseToolWindowCommand closeToolWindowCommand,
            IDocumentStatusViewModel activeDocumentStatus,
            Func<ILanguageClient> lsp, UIServiceHelper service)
            : base(state, isReadOnly, showSettingsCommand, closeToolWindowCommand, activeDocumentStatus)
        {
            _languageClient = new Lazy<ILanguageClient>(() => lsp.Invoke(), isThreadSafe: true);
            _service = service;

            Title = state.Name;
            SettingKey = nameof(EditorSettings);

            IdleTimer = new Timer(IdleTimerCallback, null, Convert.ToInt32(IdleDelay.TotalMicroseconds), Timeout.Infinite);
        }

        /// <summary>
        /// A timer that runs between keypresses to evaluate idle time; 
        /// callback is invoked if/when a configurable threshold is met, to notify the server of document changes.
        /// </summary>
        private Timer IdleTimer { get; }
        private TimeSpan IdleDelay => UIServiceHelper.Instance!.Settings.EditorSettings.IdleTimerDuration;
        private void IdleTimerCallback(object? _) => NotifyDocumentChanged();

        /// <summary>
        /// Resets the idle timer to fire a callback in <c>IdleDelay</c> milliseconds.
        /// </summary>
        private void ResetIdleTimer() => IdleTimer.Change(Convert.ToInt32(IdleDelay.TotalMilliseconds), Timeout.Infinite);

        private ILanguageClient LanguageClient => _languageClient.Value;

        private CodeDocumentState _state;
        public CodeDocumentState CodeDocumentState
        {
            get => _state;
            set
            {
                _state = value;
                DocumentState = value;
                OnPropertyChanged();
            }
        }

        protected override void OnTextChanged()
        {
            // todo notify server, etc.
        }

        public string LanguageId => _state.Language.Id;


        private WorkspaceFileUri _uri;
        public WorkspaceFileUri CodeDocumentUri
        {
            get => _uri;
            set
            {
                if (_uri != value)
                {
                    _uri = value;
                    OnPropertyChanged();
                }
            }
        }

        public override SupportedDocumentType DocumentType => SupportedDocumentType.SourceFile;

        private void NotifyDocumentChanged() => NotifyDocumentChanged(null!, null!);

        private void NotifyDocumentChanged(OmniSharp.Extensions.LanguageServer.Protocol.Models.Range? range, string? text)
        {
            // increment local version first...
            DocumentState = _state with { Version = _state.Version + 1 };

            var request = new DidChangeTextDocumentParams
            {
                TextDocument = new OptionalVersionedTextDocumentIdentifier
                {
                    Uri = DocumentState.Uri.AbsoluteLocation,
                    Version = DocumentState.Version, // ...so that the server-side latest matches local version
                },
                ContentChanges = new Container<TextDocumentContentChangeEvent>(
                new TextDocumentContentChangeEvent
                {
                    // if only Text is supplied, server considers it the document's entire content
                    Text = text ?? TextContent,
                    Range = range
                })
            };

            LanguageClient.DidChangeTextDocument(request);
        }

        private async Task<IEnumerable<Diagnostic>> RequestDiagnosticsAsync()
        {
            var report = await LanguageClient.RequestDocumentDiagnostic(new DocumentDiagnosticParams
            {
                Identifier = "RDE",
                TextDocument = new TextDocumentIdentifier
                {
                    Uri = _uri.AbsoluteLocation,
                }
            });

            if (report is IFullDocumentDiagnosticReport fullReport)
            {
                CodeDocumentState = _state.WithDiagnostics(fullReport.Items);
                return fullReport.Items;
            }
            else
            {
                return _state.Diagnostics;
            }
        }

        private async Task<IEnumerable<FoldingRange>> RequestFoldingsAsync()
        {
            var foldings = await LanguageClient.RequestFoldingRange(new FoldingRangeRequestParam
            {
                TextDocument = new TextDocumentIdentifier
                {
                    Uri = _uri.AbsoluteLocation,
                }
            });

            return foldings?.ToList() ?? [];
        }

    }
}
