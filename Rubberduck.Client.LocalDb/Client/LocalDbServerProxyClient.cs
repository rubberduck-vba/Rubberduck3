using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Client;
using Rubberduck.RPC.Proxy.LocalDbServer;
using Rubberduck.RPC.Proxy.SharedServices.Console.Commands.Parameters;
using Rubberduck.RPC.Proxy.SharedServices.Server.Commands;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.Remoting.Proxies;
using System.Text;
using System.Threading.Tasks;

namespace Rubberduck.Client.LocalDb.Client
{
    internal class LocalDbServerProxyClient : JsonRpcClientSideServerProxyService<ILocalDbServerProxyClient>, ILocalDbServerProxyClient
    {
        public LocalDbServerProxyClient(IRpcStreamFactory<NamedPipeClientStream> rpcStreamFactory) 
            : base(rpcStreamFactory)
        {
        }

        public event EventHandler<ClientInitializedParams> ClientInitialized;
        public async Task OnClientInitializedAsync(ClientInitializedParams parameter) 
            => await NotifyAsync(async proxy => await proxy.OnClientInitializedAsync(parameter));

        public event EventHandler<ClientShutdownParams> ClientShutdown;
        public async Task OnClientShutdownAsync(ClientShutdownParams parameter) 
            => await NotifyAsync(async proxy => await proxy.OnClientShutdownAsync(parameter));

        public event EventHandler RequestExit;
        public async Task OnRequestExitAsync()
            => await NotifyAsync(async proxy => await proxy.OnRequestExitAsync());

        public event EventHandler<InitializedParams> Initialized;
        public async Task OnInitializedAsync(InitializedParams parameter)
            => await NotifyAsync(async proxy => await proxy.OnInitializedAsync(parameter));

        public event EventHandler<SetTraceParams> SetTrace;
        public async Task OnSetTraceAsync(SetTraceParams parameter)
            => await NotifyAsync(async proxy => await proxy.OnSetTraceAsync(parameter));
    }
}
