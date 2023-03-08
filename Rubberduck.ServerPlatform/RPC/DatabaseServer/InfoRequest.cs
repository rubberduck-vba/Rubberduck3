using MediatR;
using Rubberduck.ServerPlatform.Model;

namespace Rubberduck.ServerPlatform.RPC.DatabaseServer
{
    public class InfoRequest : IRequest, IRequest<InfoResult>
    {
    }

    public class InfoResult
    {
        public ServerProcessInfo ServerInfo { get; set; }
    }
}
