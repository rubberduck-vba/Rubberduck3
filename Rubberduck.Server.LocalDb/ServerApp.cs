using System;
using System.Collections.Generic;
using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Proxy.SharedServices.Abstract;
using Rubberduck.Server.LocalDb.Services;

namespace Rubberduck.Server.LocalDb
{
    internal class ServerApp : RubberduckServerApp<IJsonRpcServer, LocalDbServerService, ServerCapabilities>
    {
        public ServerApp(IJsonRpcServer server, IEnumerable<IJsonRpcTarget> proxies, IServerStateService<ServerCapabilities> stateService) 
            : base(server, proxies, stateService)
        {
        }

        protected override void RegisterServerProxyEvents(LocalDbServerService proxy)
        {
            proxy.WillExit += Server_WillExit;
        }

        private void Server_WillExit(object sender, EventArgs e)
        {
            // TODO notify any connected clients, this server is going down
        }
    }
}
