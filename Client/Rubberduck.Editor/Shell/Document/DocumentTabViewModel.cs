using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using Rubberduck.UI.Command.SharedHandlers;
using Rubberduck.UI.Shell.Document;
using Rubberduck.UI.Shell.StatusBar;
using Rubberduck.UI.Windows;
using System;

namespace Rubberduck.Editor.Shell.Document
{
    /// <summary>
    /// The base implementation of a view model for a document tab that may contain anything.
    /// </summary>
    public abstract class DocumentTabViewModel : WindowViewModel, IDocumentTabViewModel
    {
        public event EventHandler<WorkspaceFileUriEventArgs> DocumentStateChanged = delegate { };

        public DocumentTabViewModel(DocumentState state, bool isReadOnly,
            ShowRubberduckSettingsCommand showSettingsCommand,
            CloseToolWindowCommand closeToolWindowCommand,
            IDocumentStatusViewModel activeDocumentStatus)
            : base(showSettingsCommand, closeToolWindowCommand)
        {
            _uri = state.Uri;
            _header = state.Name;
            _isReadOnly = isReadOnly;

            DocumentState = state;
            TextContent = state.Text;

            Status = activeDocumentStatus;
        }

        private DocumentState _state;
        public DocumentState DocumentState
        {
            get => _state;
            set
            {
                _state = value;
                OnPropertyChanged();
            }
        }


        public abstract SupportedDocumentType DocumentType { get; }

        public IDocumentStatusViewModel Status { get; }

        private Uri _uri;
        public Uri DocumentUri
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

        private string _textContent;
        public string TextContent
        {
            get => _textContent;
            set
            {
                var wasNull = _textContent is null;
                if (_textContent != value)
                {
                    _textContent = value;
                    if (!wasNull)
                    {
                        OnPropertyChanged();
                        OnTextChanged();
                    }
                }
            }
        }

        protected virtual void OnTextChanged()
        {

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

        private object? _contentControl;
        public object ContentControl
        {
            get => _contentControl ?? throw new InvalidOperationException("Content control is not set");
            set
            {
                if (_contentControl != value)
                {
                    _contentControl = value ?? throw new ArgumentNullException(nameof(value));
                    OnPropertyChanged();
                }
            }
        }
    }
}
