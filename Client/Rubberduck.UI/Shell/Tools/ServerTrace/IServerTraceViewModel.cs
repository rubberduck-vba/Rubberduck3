using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.ServerPlatform;
using Rubberduck.UI.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace Rubberduck.UI.Shell.Tools.ServerTrace
{
    public class LogMessageViewModel
    {
        public LogMessageViewModel(LogMessagePayload payload)
        {
            MessageId = payload.MessageId;
            Timestamp = payload.Timestamp;
            Message = payload.Message;
            Verbose = payload.Verbose ?? string.Empty;
            Level = payload.Level;
        }

        public int MessageId { get; }
        public DateTime Timestamp { get; }
        public string Message { get; }
        public string Verbose { get; }
        public LogLevel Level { get; }
    }

    public interface IServerTraceViewModel : IToolWindowViewModel
    {
        ICommand CopyContentCommand { get; }
        ICommand ClearContentCommand { get; }
        ICommand OpenLogFileCommand { get; }
        ICommand PauseResumeTraceCommand { get; }
        ICommand ShutdownServerCommand { get; }
        bool IsPaused { get; set; }

        void OnServerMessage(LogMessagePayload payload);

        ObservableCollection<LogMessageViewModel> LogMessages { get; }

        LogMessageFiltersViewModel Filters { get; }
    }

    public class LogMessageFiltersViewModel : ViewModelBase
    {
        private bool _trace = true;
        public bool ShowTraceItems 
        {
            get => _trace;
            set
            {
                if (_trace != value)
                {
                    _trace = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _debug = true;
        public bool ShowDebugItems 
        {
            get => _debug;
            set
            {
                if (_debug != value)
                {
                    _debug = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _info = true;
        public bool ShowInfoItems 
        {
            get => _info;
            set
            {
                if ( _info != value)
                {
                    _info = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _warn = true;
        public bool ShowWarningItems 
        {
            get => _warn;
            set
            {
                if ( _warn != value)
                {
                    _warn = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _error = true;
        public bool ShowErrorItems 
        {
            get => _error;
            set
            {
                if ( _error != value)
                {
                    _error = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
