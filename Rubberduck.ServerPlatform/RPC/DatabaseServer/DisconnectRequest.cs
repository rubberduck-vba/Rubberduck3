using MediatR;
using Rubberduck.ServerPlatform.Model;

namespace Rubberduck.ServerPlatform.RPC.DatabaseServer
{
    public class DisconnectRequest : IRequest, IRequest<DisconnectResult>
    {
        public ClientProcessInfo ClientInfo { get; set; }
    }

    public class DisconnectResult
    {
        public static DisconnectResult Success { get; } = new DisconnectResult { Disconnected = true };

        public bool Disconnected { get; set; }
    }
}
