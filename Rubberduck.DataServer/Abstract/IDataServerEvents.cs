using Rubberduck.RPC.Platform;
using System;

namespace Rubberduck.DataServer.Abstract
{
    public interface IDataServerEvents
    {
        event EventHandler OnClientConnected;
        event EventHandler OnClientDisconnected;
    }
}
