using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.ServerPlatform;
using Rubberduck.UI;
using Rubberduck.UI.Command;
using Rubberduck.UI.Services.Abstract;
using Rubberduck.UI.Shell.StatusBar;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Rubberduck.Editor.Shell.StatusBar
{
    public class ShellStatusBarViewModel : StatusBarViewModel, IShellStatusBarViewModel
    {
        private readonly IWorkDoneProgressStateService _serverProgressService;

        public ShellStatusBarViewModel(ILanguageServerConnectionStatusProvider lspConnectionStatusProvider,
            IWorkDoneProgressStateService serverProgressService,
            ShowLanguageServerTraceCommand showLanguageServerTraceCommand, 
            IDocumentStatusViewModel activeDocumentStatus)
            : base(lspConnectionStatusProvider, showLanguageServerTraceCommand)
        {
            _serverProgressService = serverProgressService;
            _activeDocumentStatus = activeDocumentStatus;

            _serverProgressService.Progress += OnServerProgress;
        }

        private string? _currentProgressToken;

        private void OnServerProgress(object? sender, ProgressEventArgs e)
        {
            if (e.Value is null) // progress token was just created
            {
                _currentProgressToken = e.Token.ToString();
                ProgressMaxValue = 100;
                return;
            }

            if (e.Token.ToString() == _currentProgressToken)
            {
                CanCancelWorkDoneProgress = e.Value.Cancellable;
                ProgressMessage = e.Value.Message;
                if (e.Value.Percentage.HasValue)
                {
                    ProgressValue = e.Value.Percentage.Value;
                }

                if (e.Value.Kind == WorkDoneProgressKind.End)
                {
                    ProgressMaxValue = 0;
                    ProgressValue = 0;
                    _currentProgressToken = null;
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
        public ICommand CancelWorkDoneProgressCommand { get; set; }

        private readonly ObservableCollection<INotificationViewModel> _notifications = [];
        public ICollection<INotificationViewModel> Notifications => _notifications;

        private readonly IDocumentStatusViewModel _activeDocumentStatus;
        public IDocumentStatusViewModel ActiveDocumentStatus => _activeDocumentStatus;
    }

    public class StatusBarViewModel : ViewModelBase, ILanguageServerStatusViewModel
    {
        private readonly ILanguageServerConnectionStatusProvider _lspConnectionStatusProvider;

        public StatusBarViewModel(ILanguageServerConnectionStatusProvider lspConnectionStatusProvider,
            ShowLanguageServerTraceCommand showLanguageServerTraceCommand)
        {
            ShowLanguageServerTraceCommand = showLanguageServerTraceCommand;
            _statusText = _serverConnectionState.ToString();
            _lspConnectionStatusProvider = lspConnectionStatusProvider;

            _lspConnectionStatusProvider.Connecting += OnLanguageServerConnecting;
            _lspConnectionStatusProvider.Connected += OnLanguageServerConnected;
            _lspConnectionStatusProvider.Disconnected += OnLanguageServerDisconnected;
        }

        private void OnLanguageServerDisconnected(object? sender, EventArgs e) => ServerConnectionState = ServerConnectionState.Disconnected;
        private void OnLanguageServerConnected(object? sender, EventArgs e) => ServerConnectionState = ServerConnectionState.Connected;
        private void OnLanguageServerConnecting(object? sender, EventArgs e) => ServerConnectionState = ServerConnectionState.Connecting;
        

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

                    IsConnected = _serverConnectionState == ServerConnectionState.Connected;
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
