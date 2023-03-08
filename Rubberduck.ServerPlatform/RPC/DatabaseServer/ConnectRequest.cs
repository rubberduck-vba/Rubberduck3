using MediatR;
using Rubberduck.ServerPlatform.Model;

namespace Rubberduck.ServerPlatform.RPC.DatabaseServer
{
    public class ConnectRequest : IRequest, IRequest<ConnectResult>
    {
        public ClientProcessInfo ClientInfo { get; set; }
    }

    public class ConnectResult
    {
        public static ConnectResult Success { get; } = new ConnectResult { Connected = true };

        public bool Connected { get; set; }
    }
}
