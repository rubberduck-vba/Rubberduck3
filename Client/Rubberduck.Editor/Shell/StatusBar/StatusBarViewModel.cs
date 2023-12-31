using Rubberduck.UI;
using Rubberduck.UI.Command;
using Rubberduck.UI.Shell.StatusBar;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Rubberduck.Editor.Shell.StatusBar
{
    public class ShellStatusBarViewModel : StatusBarViewModel, IShellStatusBarViewModel
    {
        public ShellStatusBarViewModel(ShowLanguageServerTraceCommand showLanguageServerTraceCommand, IDocumentStatusViewModel activeDocumentStatus)
            : base(showLanguageServerTraceCommand)
        {
            _activeDocumentStatus = activeDocumentStatus;
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
        public ICommand CancelWorkDoneProgressCommand { get; set; }

        private readonly ObservableCollection<INotificationViewModel> _notifications = [];
        public ICollection<INotificationViewModel> Notifications => _notifications;

        private readonly IDocumentStatusViewModel _activeDocumentStatus;
        public IDocumentStatusViewModel ActiveDocumentStatus => _activeDocumentStatus;
    }

    public class StatusBarViewModel : ViewModelBase, ILanguageServerStatusViewModel
    {
        public StatusBarViewModel(ShowLanguageServerTraceCommand showLanguageServerTraceCommand)
        {
            ShowLanguageServerTraceCommand = showLanguageServerTraceCommand;
            _statusText = _serverConnectionState.ToString();
        }

        private ServerConnectionState _serverConnectionState = ServerConnectionState.Disconnected;
        public ServerConnectionState ServerConnectionState
        {
            get => _serverConnectionState;
            set
            {
                if (_serverConnectionState != value)
                {
                    _serverConnectionState = value;
                    OnPropertyChanged();

                    StatusText = _serverConnectionState.ToString();
                }
            }
        }

        private bool _isConnected;
        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                if (_isConnected != value)
                {
                    _isConnected = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand ShowLanguageServerTraceCommand { get; }

        private string _statusText;
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
    }
}
