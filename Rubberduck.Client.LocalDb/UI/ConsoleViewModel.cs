using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Rubberduck.Client.LocalDb.UI.Commands;
using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Proxy.LocalDbServer;
using Rubberduck.RPC.Proxy.SharedServices.Console.Commands.Parameters;
using Rubberduck.RPC.Proxy.SharedServices.Console.Configuration;
using Rubberduck.RPC.Proxy.SharedServices.Server.Commands;
using Rubberduck.UI;
using Rubberduck.UI.Abstract;
using Rubberduck.UI.Command;

namespace Rubberduck.Client.LocalDb
{
    public sealed class TraceSettingValue : ITraceSettingValue
    {
        public TraceSettingValue(string value)
        {
            Name = value; // TODO localize
            Value = value;
        }

        public string Name { get; }
        public string Value { get; }
    }

    public sealed class ConsoleViewModel : ViewModelBase, IConsoleViewModel
    {
        private static readonly IDictionary<string, TraceSettingValue> _traceSettings = new Dictionary<string, TraceSettingValue>
        {
            [Constants.TraceValue.Off] = new TraceSettingValue(Constants.TraceValue.Off),
            [Constants.TraceValue.Messages] = new TraceSettingValue(Constants.TraceValue.Messages),
            [Constants.TraceValue.Verbose] = new TraceSettingValue(Constants.TraceValue.Verbose),
        };

        public ConsoleViewModel(ILocalDbServerProxyClient server)
        {
            server.Message += OnConsoleMessage;

            ConsoleContent = new ObservableCollection<IConsoleMesssageViewModel>();

            ClearCommand = new DelegateCommand(null, o => ConsoleContent.Clear());

            ShutdownCommand = new AsyncDelegateCommand(o => server.ShutdownClientAsync(o as ClientShutdownParams));
            CopyCommand = new CopyCommand(server);
            SaveAsCommand = new SaveAsCommand(server, new FileNameProvider());

            var enableParams = new SetEnabledParams { Value = true };
            var disableParams = new SetEnabledParams { Value = false };

            PauseTraceCommand = new AsyncDelegateCommand(_ => server.OnStopTraceAsync());
            ResumeTraceCommand = new AsyncDelegateCommand(_ => server.OnResumeTraceAsync());
            SetTraceCommand = new AsyncDelegateCommand(o => server.OnSetTraceAsync(o as SetTraceParams));

            _trace = Constants.TraceValue.Verbose;

            TraceValues = _traceSettings.Values;
            SelectedTraceValue = _traceSettings[Trace];
        }

        private void OnConsoleMessage(object sender, ConsoleMessage e)
        {
            ConsoleContent.Add(new ConsoleMessageViewModel(e));
            OnPropertyChanged(nameof(ConsoleContent));
        }

        public ObservableCollection<IConsoleMesssageViewModel> ConsoleContent { get; }

        private string _searchString;
        public string SearchString
        {
            get => _searchString;
            set
            {
                if (_searchString != value)
                {
                    _searchString = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isTraceActive;
        public bool IsTraceActive 
        { 
            get => _isTraceActive;
            set
            {
                if (_isTraceActive != value)
                {
                    _isTraceActive = value;
                    OnPropertyChanged();
                }
            }
        }

        public IEnumerable<ITraceSettingValue> TraceValues { get; }

        private ITraceSettingValue _selectedTraceValue;
        public ITraceSettingValue SelectedTraceValue
        {
            get => _selectedTraceValue;
            set
            {
                if (_selectedTraceValue != value)
                {
                    _selectedTraceValue = value;
                    Trace = _selectedTraceValue.Value;
                    OnPropertyChanged();
                }
            }
        }


        private string _trace;
        public string Trace
        {
            get => _trace;
            set
            {
                if (_trace != value)
                {
                    _trace = value;
                    OnPropertyChanged();
                    SetTraceCommand.Execute(value);
                }
            }
        }

        private string _serverName;
        public string ServerName
        {
            get => _serverName;
            set
            {
                if (_serverName != value)
                {
                    _serverName = value;
                    OnPropertyChanged();
                }
            }
        }

        private TimeSpan _upTime;
        public TimeSpan Uptime
        {
            get => _upTime;
            set
            {
                _upTime = value;
                OnPropertyChanged();
            }
        }

        private int _totalInbount;
        public int TotalInbound
        {
            get => _totalInbount;
            set
            {
                if (_totalInbount != value)
                {
                    _totalInbount = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _totalOutbound;
        public int TotalOutbound
        {
            get => _totalOutbound;
            set
            {
                if (_totalOutbound != value)
                {
                    _totalOutbound = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _clientConnections;
        public int ClientConnections
        {
            get => _clientConnections;
            set
            {
                if (_clientConnections != value)
                {
                    _clientConnections = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                if (_statusMessage != value)
                {
                    _statusMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand ShutdownCommand { get ; }
        public ICommand ClearCommand { get; }
        public ICommand CopyCommand { get; }
        public ICommand SaveAsCommand { get; }
        public ICommand PauseTraceCommand { get; }
        public ICommand ResumeTraceCommand { get; }
        public ICommand SetTraceCommand { get; }
        public ICommand SearchCommand { get; }
    }
}
