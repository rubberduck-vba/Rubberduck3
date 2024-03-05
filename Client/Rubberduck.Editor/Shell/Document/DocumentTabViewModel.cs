using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.Editor.Shell.StatusBar;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using Rubberduck.InternalApi.Services;
using Rubberduck.UI.Command.SharedHandlers;
using Rubberduck.UI.Services;
using Rubberduck.UI.Shell.Document;
using Rubberduck.UI.Shell.StatusBar;
using Rubberduck.UI.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Rubberduck.Editor.Shell.Document
{
    /// <summary>
    /// The base implementation of a view model for a document tab that may contain anything.
    /// </summary>
    public abstract class DocumentTabViewModel : WindowViewModel, IDocumentTabViewModel
    {
        private Func<ILanguageClient> _lsp;
        private DocumentState _state;

        public DocumentTabViewModel(DocumentState state, bool isReadOnly,
            ShowRubberduckSettingsCommand showSettingsCommand,
            CloseToolWindowCommand closeToolWindowCommand,
            IDocumentStatusViewModel activeDocumentStatus,
            Func<ILanguageClient> lsp)
            : base(showSettingsCommand, closeToolWindowCommand)
        {
            _state = state;
            _uri = state.Uri;
            _language = state.Language.Id;
            _header = state.Name;
            _isReadOnly = isReadOnly;

            _lsp = lsp;

            Title = state.Name;
            TextContent = state.Text;

            Status = activeDocumentStatus;
        }

        public DocumentState DocumentState => _state;

        public abstract SupportedDocumentType DocumentType { get; }

        public virtual async Task<IEnumerable<Diagnostic>> RequestDiagnosticsAsync()
        {
            var report = await _lsp.Invoke().RequestDocumentDiagnostic(new DocumentDiagnosticParams
            {
                Identifier = "RDE",
                TextDocument = new TextDocumentIdentifier
                {
                    Uri = _uri.AbsoluteLocation.LocalPath,
                }
            });

            if (report is IFullDocumentDiagnosticReport fullReport)
            {
                _state = _state.WithDiagnostics(fullReport.Items);
                return fullReport.Items;
            }
            else
            {
                return _state.Diagnostics;
            }
        }

        public virtual async Task<IEnumerable<FoldingRange>> RequestFoldingsAsync()
        {
            var foldings = await _lsp.Invoke().RequestFoldingRange(new FoldingRangeRequestParam
            {
                TextDocument = new TextDocumentIdentifier
                {
                    Uri = _uri.AbsoluteLocation.LocalPath,
                }
            });

            return foldings?.ToList() ?? [];
        }

        public IDocumentStatusViewModel Status { get; }

        private WorkspaceUri _uri;
        public WorkspaceUri DocumentUri
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

        private string _language;
        public string Language
        {
            get => _language;
            set
            {
                if (_language != value)
                {
                    _language = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _textContent;
        public string TextContent
        {
            get => _textContent;
            set
            {
                if (_textContent != value)
                {
                    _textContent = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isReadOnly;
        public bool IsReadOnly
        {
            get => _isReadOnly;
            set
            {
                if (_isReadOnly != value)
                {
                    _isReadOnly = value;
                    OnPropertyChanged();
                }
            }
        }


        private object _header;
        public object Header
        {
            get => _header;
            set
            {
                if (_header != value)
                {
                    _header = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isSelected;
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

        private string _documentFont = "Calibri";
        public string DocumentFont
        {
            get => _documentFont;
            set
            {
                if (_documentFont != value)
                {
                    _documentFont = value;
                    OnPropertyChanged();
                }
            }
        }

        private object _content;
        public object ContentControl
        {
            get => _content;
            set
            {
                if (_content != value)
                {
                    _content = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
