using Rubberduck.InternalApi.Common;
using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Proxy.LocalDbServer;
using Rubberduck.RPC.Proxy.SharedServices;
using Rubberduck.RPC.Proxy.SharedServices.Console.Configuration;
using Rubberduck.UI;
using Rubberduck.UI.Abstract;
using System;
using System.Threading;

namespace Rubberduck.Client.LocalDb
{
    public class ConsoleMessageViewModel : ViewModelBase, IConsoleMesssageViewModel, IExportable
    {
        public ConsoleMessageViewModel(ConsoleMessage args)
        {
            Id = args.Id;
            Timestamp = args.Timestamp;
            IsError = args.IsError;
            Exception = args.Exception;
            Level = (int)args.Level;
            Message = args.Message;
            Verbose = args.Verbose;
        }

        public int Id { get; }
        public DateTime Timestamp { get; }
        public bool IsError { get; }
        public bool IsWarning => Level == (int)ServerLogLevel.Warn;
        public Exception Exception { get; }
        public int Level { get; }
        public string Message { get; }
        public string Verbose { get; }

        public object[] ToArray()
        {
            return new object[]
            {
                Id,
                Timestamp,
                IsError,
                Exception,
                Level,
                Message,
                Verbose
            };
        }

        public string ToClipboardString()
        {
            return string.Join("\t", ToArray());
        }
    }

    public class ServerStatusViewModel : ViewModelBase, IServerStatusViewModel
    {
        private readonly Timer _timer;

        public ServerStatusViewModel(IJsonRpcServer server, ILocalDbServerEvents events)
        {
            _timer = new Timer(_ =>
            {
                Uptime = server.Info.UpTime ?? TimeSpan.Zero;
                TotalInbound = server.Info.MessagesReceived;
                TotalOutbound = server.Info.MessagesSent;
                ClientConnections = server.Info.ClientsCount;
                IsAlive = server.Info.IsAlive;
            }, null, 0, 1000);

            ServerName = server.Info.Name;

            events.ClientConnected += (o, e) => Interlocked.Increment(ref _clientConnections);
            events.ClientDisconnected += (o, e) => Interlocked.Decrement(ref _clientConnections);
        }

        public string ServerName { get; }

        private TimeSpan _uptime;
        public TimeSpan Uptime 
        { 
            get => _uptime; 
            private set
            {
                _uptime = value;
                OnPropertyChanged();
            }
        }

        private int _totalInbound = 0;
        public int TotalInbound
        {
            get => _totalInbound;
            private set
            {
                if (_totalInbound != value) 
                {
                    _totalInbound = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _totalOutbound = 0;
        public int TotalOutbound
        {
            get => _totalOutbound;
            private set
            {
                if (_totalOutbound != value) 
                {
                    _totalOutbound = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _clientConnections = 0;
        public int ClientConnections 
        {
            get => _clientConnections;
            private set
            {
                _clientConnections = value;
                OnPropertyChanged();
            }
        }

        private bool _isAlive;
        public bool IsAlive
        {
            get => _isAlive;
            private set
            {
                if (_isAlive != value)
                {
                    _isAlive = value;
                    StatusMessage = _isAlive ? "Ready." : "Disconnected.";
                    OnPropertyChanged();
                }
            }
        }

        private string _statusMessage;
        public string StatusMessage 
        {
            get => _statusMessage;
            private set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        public void Dispose()
        {
            _timer.Dispose();
        }
    }

    public class MainWindowViewModel : ViewModelBase, IDataServerMainWindowViewModel
    {
        public MainWindowViewModel(IConsoleViewModel console, IServerStatusViewModel status)
        {
            Console = console;
            Status = status;
        }

        public IConsoleViewModel Console { get; }
        public IServerStatusViewModel Status { get; }
    }
}
