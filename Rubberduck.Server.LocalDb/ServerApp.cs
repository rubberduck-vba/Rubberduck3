using System;
using System.Collections.Generic;
using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Proxy.LocalDbServer;
using Rubberduck.RPC.Proxy.SharedServices.Abstract;

namespace Rubberduck.Server.LocalDb
{
    internal class ServerApp : RubberduckServerApp<IJsonRpcServer, LocalDbServerProxyService, LocalDbServerCapabilities>
    {
        public ServerApp(IJsonRpcServer server, IEnumerable<IJsonRpcTarget> proxies, IServerStateService<LocalDbServerCapabilities> stateService) 
            : base(server, proxies, stateService)
        {
            
        }

        protected override void RegisterServerProxyEvents(LocalDbServerProxyService proxy)
        {
            proxy.WillExit += Server_WillExit;
        }

        private void Server_WillExit(object sender, EventArgs e)
        {
            // TODO notify any connected clients, this server is going down
        }
    }
}
