using System;

namespace Rubberduck.UI.Xaml.Dependencies.Controls.ServerStatus
{
    public interface IServerStatusViewModel : IDisposable
    {
        string ServerName { get; }
        TimeSpan Uptime { get; }
        int TotalInbound { get; }
        int TotalOutbound { get; }
        int ClientConnections { get; }
        string StatusMessage { get; }

        bool IsAlive { get; }
    }
}
