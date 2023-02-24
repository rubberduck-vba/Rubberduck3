using Rubberduck.RPC.Proxy.LocalDbServer;
using Rubberduck.UI;
using Rubberduck.UI.Abstract;
using System;
using System.Threading;

namespace Rubberduck.Client.LocalDb
{
    public class ServerStatusViewModel : ViewModelBase, IServerStatusViewModel
    {
        private readonly Timer _timer;

        public ServerStatusViewModel(ILocalDbServerProxyClient server)
        {
            //_timer = new Timer(_ =>
            //{
            //    var info = server.OnRequestServerInfoAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            //    Uptime = info.UpTime ?? TimeSpan.Zero;
            //    TotalInbound = info.MessagesReceived;
            //    TotalOutbound = info.MessagesSent;
            //    ClientConnections = info.ClientsCount;
            //    IsAlive = info.IsAlive;

            //    ServerName = info.Name;
            //}, null, 1000, 1000);
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
}
