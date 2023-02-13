using Rubberduck.RPC.Proxy.SharedServices.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Server.Configuration;
using System;
using System.Threading.Tasks;

namespace Rubberduck.RPC.Proxy.SharedServices.Server.Commands
{
    public class DisconnectClientCommand : ShutdownCommand<ClientShutdownParams>
    {
        public DisconnectClientCommand(Action<ClientShutdownParams> shutdownAction, IServerLogger logger, GetServerOptionsAsync<SharedServerCapabilities> getConfiguration, GetServerStateInfoAsync getServerState) 
            : base(shutdownAction, logger, getConfiguration, getServerState)
        {
        }

        public override string Description { get; } = "Disconnects a client from the server.";

        protected override Task ExecuteInternalAsync(ClientShutdownParams parameter)
        {
            throw new NotImplementedException();
        }
    }
}
