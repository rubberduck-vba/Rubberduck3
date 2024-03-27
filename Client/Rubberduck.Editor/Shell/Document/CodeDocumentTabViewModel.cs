using AsyncAwaitBestPractices;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using Rubberduck.InternalApi.Settings.Model.Editor;
using Rubberduck.UI.Command.SharedHandlers;
using Rubberduck.UI.Services;
using Rubberduck.UI.Shell.Document;
using Rubberduck.UI.Shell.StatusBar;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.Editor.Shell.Document
{
    /// <summary>
    /// The base view model for a type of document tab that contains code managed by a language server.
    /// </summary>
    public abstract class CodeDocumentTabViewModel : DocumentTabViewModel, ICodeDocumentTabViewModel
    {
        public event EventHandler CodeDocumentStateChanged = delegate { };

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
            _uri = state.Uri;

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
        private void IdleTimerCallback(object? _)
        {
            Status.IsWriting = false;
            DisableIdleTimer();
            NotifyDocumentChangedAsync()
                .ContinueWith(t => Task.Delay(IdleDelay))
                .ContinueWith(t =>
                {
                    RequestDiagnosticsAsync().SafeFireAndForget();
                    RequestFoldingsAsync().SafeFireAndForget();
                }).SafeFireAndForget();
        }

        /// <summary>
        /// Resets the idle timer to fire a callback in <c>IdleDelay</c> milliseconds.
        /// </summary>
        /// <remarks>
        /// Invoked at every keypress.
        /// </remarks>
        private void ResetIdleTimer()
        {
            IdleTimer.Change(Convert.ToInt32(IdleDelay.TotalMilliseconds), Timeout.Infinite);
        }

        private void DisableIdleTimer() => IdleTimer.Change(Timeout.Infinite, Timeout.Infinite);

        private ILanguageClient LanguageClient => _languageClient.Value;

        private CodeDocumentState _state;
        public CodeDocumentState CodeDocumentState
        {
            get => _state;
            set
            {
                _state = value;
                OnPropertyChanged();
                CodeDocumentStateChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public override DocumentState DocumentState 
        { 
            get => _state; 
            set => CodeDocumentState = value as CodeDocumentState ?? throw new InvalidOperationException(); 
        }

        public override Uri DocumentUri 
        {
            get => _uri;
            set => CodeDocumentUri = value as WorkspaceFileUri ?? throw new InvalidOperationException();
        }

        protected override void OnTextChanged()
        {
            Status.IsWriting = true;
            ResetIdleTimer();
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

        private async Task NotifyDocumentChangedAsync() => await NotifyDocumentChangedAsync(null!, null!);

        private async Task NotifyDocumentChangedAsync(OmniSharp.Extensions.LanguageServer.Protocol.Models.Range? range, string? text)
        {
            Status.ProgressMessage = "Processing changes...";
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

            _service.LogDebug($"Notifying server of document changes.", $"DocumentId: {DocumentState.Id} Version: {DocumentState.Version}");
            LanguageClient.DidChangeTextDocument(request);

            await RequestFoldingsAsync();
            await RequestDiagnosticsAsync();
            Status.ProgressMessage = null;
        }

        private async Task RequestDiagnosticsAsync()
        {
            var request = new DocumentDiagnosticParams
            {
                Identifier = "RDE",
                TextDocument = new TextDocumentIdentifier
                {
                    Uri = _uri.AbsoluteLocation,
                }
            };

            _service.LogDebug($"Requesting document diagnostics.");
            var report = await LanguageClient.RequestDocumentDiagnostic(request);

            if (report is IFullDocumentDiagnosticReport fullReport)
            {
                _service.LogDebug($"Received {fullReport.Items.Count()} diagnostics.");
                CodeDocumentState = _state.WithDiagnostics(fullReport.Items);
            }
            else
            {
                _service.LogDebug($"Received a diagnostic report that was not a IFullDocumentDiagnosticReport.", $"Report type : {report?.GetType().Name ?? "(null)"}");
            }
        }

        private async Task RequestFoldingsAsync()
        {
            var request = new FoldingRangeRequestParam
            {
                TextDocument = new TextDocumentIdentifier
                {
                    Uri = _uri.AbsoluteLocation,
                }
            };

            _service.LogDebug($"Requesting document folding ranges.");
            var foldings = await LanguageClient.RequestFoldingRange(request);
            if (foldings is not null)
            {
                _service.LogDebug($"Received {foldings.Count()} document folding ranges.");
                CodeDocumentState = _state.WithFoldings(foldings);
            }
            else
            {
                _service.LogDebug($"Received a null response for folding ranges.");
            }
        }
    }
}
