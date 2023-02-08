using System;
using System.IO.Pipes;
using System.Threading.Tasks;
using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Client;
using Rubberduck.RPC.Platform.Model;
using Rubberduck.RPC.Proxy.SharedServices.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Server.Commands;
using Rubberduck.RPC.Proxy.SharedServices.Server.Model;

namespace Rubberduck.RPC.Proxy.LocalDbServer
{
    public class LocalDbServerProxyClient : JsonRpcClientSideServerProxyService<ILocalDbServerProxyClient>, ILocalDbServerProxyClient
    {
        public IServerLogger Logger { get; set; }

        public LocalDbServerCapabilities Configuration { get; set; }


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

        public async Task<InitializeResult<LocalDbServerCapabilities>> InitializeClientAsync(InitializeParams<LocalDbServerCapabilities> parameter)
            => await RequestAsync(async proxy => await proxy.InitializeClientAsync(parameter));

        public async Task ShutdownClientAsync(ClientShutdownParams parameter)
            => await NotifyAsync(async proxy => await proxy.OnClientShutdownAsync(parameter));

        public async Task<ServerState> OnRequestServerInfoAsync()
            => await RequestAsync(async proxy => await proxy.OnRequestServerInfoAsync());
    }
}
