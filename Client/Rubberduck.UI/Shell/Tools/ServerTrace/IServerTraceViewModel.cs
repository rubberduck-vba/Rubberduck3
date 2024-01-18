using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.ServerPlatform;
using Rubberduck.UI.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
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

            _payload = payload.ToString();
        }

        public int MessageId { get; }
        public DateTime Timestamp { get; }
        public string Message { get; }
        public string Verbose { get; }
        public LogLevel Level { get; }

        private readonly string _payload;
        public string AsJsonString() => _payload;
    }

    public interface IServerTraceViewModel : IToolWindowViewModel
    {
        ICommand CopyContentCommand { get; }
        ICommand ClearContentCommand { get; }
        ICommand OpenLogFileCommand { get; }
        ICommand PauseResumeTraceCommand { get; }
        ICommand ShutdownServerCommand { get; }
        ICommand ClearFiltersCommand { get; }
        bool IsPaused { get; set; }

        void OnServerMessage(LogMessagePayload payload);

        ICollectionView LogMessages { get; }

        LogMessageFiltersViewModel Filters { get; }
    }

    public class LogMessageFiltersViewModel : ViewModelBase
    {
        private readonly Dictionary<LogLevel, bool> _filters = new()
        {
            [LogLevel.Trace] = true,
            [LogLevel.Debug] = true,
            [LogLevel.Information] = true,
            [LogLevel.Warning] = true,
            [LogLevel.Error] = true,
        };

        public event EventHandler FiltersChanged = delegate { };
        private void OnFiltersChanged()
        {
            FiltersChanged?.Invoke(this, EventArgs.Empty);
            IsFiltered = _filters.Any(e => !e.Value);
        }

        public void Clear()
        {
            ShowTraceItems = true;
            ShowDebugItems = true;
            ShowInfoItems = true;
            ShowWarningItems = true;
            ShowErrorItems = true;
        }

        public LogLevel[] Filters => _filters.Where(kvp => kvp.Value).Select(kvp => kvp.Key).ToArray();

        private bool _isFiltered;
        public bool IsFiltered
        {
            get => _isFiltered;
            set
            {
                if (_isFiltered != value)
                {
                    _isFiltered = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool ShowTraceItems 
        {
            get => _filters[LogLevel.Trace];
            set
            {
                if (_filters[LogLevel.Trace] != value)
                {
                    _filters[LogLevel.Trace] = value;
                    OnPropertyChanged();
                    OnFiltersChanged();
                }
            }
        }

        public bool ShowDebugItems 
        {
            get => _filters[LogLevel.Debug];
            set
            {
                if (_filters[LogLevel.Debug] != value)
                {
                    _filters[LogLevel.Debug] = value;
                    OnPropertyChanged();
                    OnFiltersChanged();
                }
            }
        }

        public bool ShowInfoItems 
        {
            get => _filters[LogLevel.Information];
            set
            {
                if (_filters[LogLevel.Information] != value)
                {
                    _filters[LogLevel.Information] = value;
                    OnPropertyChanged();
                    OnFiltersChanged();
                }
            }
        }

        public bool ShowWarningItems 
        {
            get => _filters[LogLevel.Warning];
            set
            {
                if (_filters[LogLevel.Warning] != value)
                {
                    _filters[LogLevel.Warning] = value;
                    OnPropertyChanged();
                    OnFiltersChanged();
                }
            }
        }

        public bool ShowErrorItems 
        {
            get => _filters[LogLevel.Error];
            set
            {
                if (_filters[LogLevel.Error] != value)
                {
                    _filters[LogLevel.Error] = value;
                    OnPropertyChanged();
                    OnFiltersChanged();
                }
            }
        }
    }
}
