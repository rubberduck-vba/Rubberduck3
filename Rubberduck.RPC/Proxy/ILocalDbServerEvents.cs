using System;

namespace Rubberduck.RPC.Proxy
{
    public interface ILocalDbServerEvents
    {
        event EventHandler OnClientConnected;
        event EventHandler OnClientDisconnected;
    }
}
