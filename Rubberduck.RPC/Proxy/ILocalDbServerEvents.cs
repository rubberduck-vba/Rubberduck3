using System;

namespace Rubberduck.RPC.Proxy
{
    public interface ILocalDbServerEvents
    {
        event EventHandler ShutdownSignal;
        event EventHandler ClientConnected;
        event EventHandler ClientDisconnected;
    }
}
