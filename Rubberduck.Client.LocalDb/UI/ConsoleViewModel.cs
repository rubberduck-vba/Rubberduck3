using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Proxy.SharedServices;
using Rubberduck.RPC.Proxy.SharedServices.Console.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Console.Commands;
using Rubberduck.RPC.Proxy.SharedServices.Console.Configuration;
using Rubberduck.RPC.Proxy.SharedServices.Server.Configuration;
using Rubberduck.UI;
using Rubberduck.UI.Abstract;
using Rubberduck.UI.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

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

        public ConsoleViewModel(ServerService<SharedServerCapabilities> server, IServerConsoleService<SharedServerCapabilities> console, ServerConsoleCommands commands)
        {
            console.Message += OnConsoleMessage;

            ConsoleContent = new ObservableCollection<IConsoleMesssageViewModel>();

            ClearCommand = new DelegateCommand(null, o => ConsoleContent.Clear());
            /*
            ShutdownCommand = server.Commands.ExitCommand;
            CopyCommand = null;
            SaveAsCommand = null;
            PauseTraceCommand = new DelegateCommand(null, server.ServerConsole.Commands.SetTraceCommand);
            ResumeTraceCommand = resumeTraceCommand;
            SetTraceCommand = setTraceCommand;
            */
            _trace = console.Configuration.ConsoleOptions.Trace.ToString();
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

        private int _rpcPort;
        public int RpcPort
        {
            get => _rpcPort;
            set
            {
                if (_rpcPort != value)
                {
                    _rpcPort = value;
                    OnPropertyChanged();
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
