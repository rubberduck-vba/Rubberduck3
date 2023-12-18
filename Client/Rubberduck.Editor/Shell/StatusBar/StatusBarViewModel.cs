using Rubberduck.UI;
using Rubberduck.UI.Shell.StatusBar;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Rubberduck.Editor.Shell.StatusBar
{
    public class ShellStatusBarViewModel : ViewModelBase, IShellStatusBarViewModel
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

        private readonly ObservableCollection<INotificationViewModel> _notifications = new();
        public ICollection<INotificationViewModel> Notifications => _notifications;
    }

    public class StatusBarViewModel : ViewModelBase
    {
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
                }
            }
        }

        public bool ShowDocumentStatusItems { get; set; } = false;

        private string _serverStateText = ServerConnectionState.Disconnected.ToString(); // TODO localize
        public string StatusText
        {
            get => _serverStateText;
            set
            {
                if (value != _serverStateText)
                {
                    _serverStateText = value;
                    OnPropertyChanged();
                }
            }
        }

        public int DocumentLines { get; set; }
        public int DocumentLength { get; set; }
        public int CaretOffset { get; set; }
        public int CaretLine { get; set; }
        public int CaretColumn { get; set; }
        public int IssuesCount { get; set; }
        public int ProgressValue { get; set; }
        public int ProgressMaxValue { get; set; }
    }
}
