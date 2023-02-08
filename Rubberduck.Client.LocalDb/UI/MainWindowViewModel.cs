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

        public ServerStatusViewModel(ILocalDbServerProxyClient server)
        {
            _timer = new Timer(_ =>
            {
                var info = server.OnRequestServerInfoAsync().ConfigureAwait(false).GetAwaiter().GetResult();

                Uptime = info.UpTime ?? TimeSpan.Zero;
                TotalInbound = info.MessagesReceived;
                TotalOutbound = info.MessagesSent;
                ClientConnections = info.ClientsCount;
                IsAlive = info.IsAlive;

                ServerName = info.Name;
            }, null, 0, 1000);
        }

        public string ServerName { get; private set; }

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
