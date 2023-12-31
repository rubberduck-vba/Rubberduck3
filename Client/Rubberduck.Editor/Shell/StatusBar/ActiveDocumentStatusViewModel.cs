using Rubberduck.UI;
using Rubberduck.UI.Shell.Document;
using Rubberduck.UI.Shell.StatusBar;
using System.Windows.Input;

namespace Rubberduck.Editor.Shell.StatusBar
{
    public class ActiveDocumentStatusViewModel : ViewModelBase, IDocumentStatusViewModel
    {
        private string _statusText = "Ready";
        public string StatusText
        {
            get => _statusText;
            set
            {
                if (_statusText != value)
                {
                    _statusText = value;
                    OnPropertyChanged();
                }
            }
        }

        private string? _progressMessage;
        public string? ProgressMessage
        {
            get => _progressMessage;
            set
            {
                if (_progressMessage != value)
                {
                    _progressMessage = value;
                    OnPropertyChanged();
                }
            }
        }
        private int _progressValue;
        public int ProgressValue
        {
            get => _progressValue;
            set
            {
                if (_progressValue != value)
                {
                    _progressValue = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _progressMaxValue;
        public int ProgressMaxValue
        {
            get => _progressMaxValue;
            set
            {
                if (_progressMaxValue != value)
                {
                    _progressMaxValue = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _canCancelWorkDoneProgress;
        public bool CanCancelWorkDoneProgress
        {
            get => _canCancelWorkDoneProgress;
            set
            {
                if (_canCancelWorkDoneProgress != value)
                {
                    _canCancelWorkDoneProgress = value;
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
        public ICommand CancelWorkDoneProgressCommand { get; set; }

        private SupportedDocumentType _documentType;
        public SupportedDocumentType DocumentType
        {
            get => _documentType;
            set
            {
                if (_documentType != value)
                {
                    _documentType = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _documentName;
        public string DocumentName
        {
            get => _documentName;
            set
            {
                if (_documentName != value)
                {
                    _documentName = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _documentLength;
        public int DocumentLength
        {
            get => _documentLength;
            set
            {
                if (_documentLength != value)
                {
                    _documentLength = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _documentLines;
        public int DocumentLines
        {
            get => _documentLines;
            set
            {
                if (_documentLines != value)
                {
                    _documentLines = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _caretOffset;
        public int CaretOffset
        {
            get => _caretOffset;
            set
            {
                if (_caretOffset != value)
                {
                    _caretOffset = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _caretLine;
        public int CaretLine
        {
            get => _caretLine;
            set
            {
                if (_caretLine != value)
                {
                    _caretLine = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _caretColumn;
        public int CaretColumn
        {
            get => _caretColumn;
            set
            {
                if (_caretColumn != value)
                {
                    _caretColumn = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
