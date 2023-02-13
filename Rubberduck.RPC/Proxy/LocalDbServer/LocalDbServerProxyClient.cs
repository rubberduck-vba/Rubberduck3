using System;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Client;
using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Platform.Model;
using Rubberduck.RPC.Proxy.SharedServices.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Server.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Server.Commands;
using Rubberduck.RPC.Proxy.SharedServices.Server.Model;
using StreamJsonRpc;

namespace Rubberduck.RPC.Proxy.LocalDbServer
{
    [JsonRpcSource]
    public class LocalDbServerProxyClient : JsonRpcClientSideServerProxyService<IServerProxy<LocalDbServerCapabilities, InitializeParams<LocalDbServerCapabilities>>>, ILocalDbServerProxyClient
    {
        public IServerLogger Logger { get; set; }

        public LocalDbServerCapabilities Configuration { get; set; }


        public LocalDbServerProxyClient(IRpcStreamFactory<NamedPipeClientStream> rpcStreamFactory) 
            : base(rpcStreamFactory)
        {
        }

        public event EventHandler<ClientInitializedParams> ClientInitialized;
        public event EventHandler<ClientShutdownParams> ClientShutdown;
        public event EventHandler RequestExit;
        public event EventHandler<InitializedParams> Initialized;

        public async Task OnClientInitializedAsync(ClientInitializedParams parameter) 
            => await Task.Run(() => ClientInitialized?.Invoke(this, parameter));

        public async Task OnRequestExitAsync()
            => await Task.Run(() => RequestExit?.Invoke(this, EventArgs.Empty));

        public async Task OnInitializedAsync(InitializedParams parameter)
            => await Task.Run(() => Initialized?.Invoke(this, parameter));

        [RubberduckSP(JsonRpcMethods.ServerProxyRequests.Shared.Server.Initialize)]
        public async Task<InitializeResult<LocalDbServerCapabilities>> InitializeClientAsync(InitializeParams<LocalDbServerCapabilities> parameter, CancellationToken token)
            => await RequestAsync(async proxy => await proxy.InitializeAsync(parameter, token));

        [RubberduckSP(JsonRpcMethods.ServerProxyRequests.Shared.Server.Info)]
        public async Task<ServerState> OnRequestServerInfoAsync(CancellationToken token)
            => await RequestAsync(async proxy => await proxy.RequestServerInfoAsync(token));

        [RubberduckSP(JsonRpcMethods.ServerProxyRequests.Shared.Server.Shutdown)]
        public async Task OnClientShutdownAsync(ClientShutdownParams parameter)
            => await Task.Run(() => ClientShutdown?.Invoke(this, parameter));
    }
}
