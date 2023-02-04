using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Proxy.SharedServices.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Server.Configuration;
using System;

namespace Rubberduck.RPC.Proxy.SharedServices.Server.Abstract
{
    /// <summary>
    /// A common base class for all server proxy services that expose the server-side RPC targets.
    /// </summary>
    /// <typeparam name="TOptions">The class type of the configuration options for this service.</typeparam>
    /// <typeparam name="TCommands">The class type of the class that exposes the commands for this service.</typeparam>
    public abstract class ServerProxyService<TOptions, TCommands> : IConfigurableServerProxy<TOptions, IServerProxyClient>, IJsonRpcTarget
        where TOptions : SharedServerCapabilities, new()
        where TCommands : class
    {
        protected ServerProxyService(IServerLogger logger, IServerStateService<TOptions> serverStateService)
        {
            Logger = logger;
            ServerStateService = serverStateService;
        }

        public TOptions Configuration => ServerStateService.Configuration;
        public abstract TCommands Commands { get; }
        public virtual IServerLogger Logger { get; }

        protected IServerStateService<TOptions> ServerStateService { get; }

        private IServerProxyClient _client;
        public IServerProxyClient ClientProxy 
        { 
            get => _client;
            private set
            {
                if (_client != null)
                {
                    throw new InvalidOperationException("client proxy is being set twice.");
                }

                _client = value;
                _client.Initialized += RpcClientInitialized;
                _client.SetTrace += RpcClientSetTrace;
                _client.RequestExit += RpcClientRequestExit;
            }
        }

        public Type ClientProxyType { get; } = typeof(IServerProxyClient);

        abstract protected void RpcClientSetTrace(object sender, Console.Commands.Parameters.SetTraceParams e);
        abstract protected void RpcClientRequestExit(object sender, EventArgs e);
        abstract protected void RpcClientInitialized(object sender, Commands.InitializedParams e);

        public void SetClientProxy<T>(T proxy) where T : class
        {
            ClientProxy = (IServerProxyClient)proxy;
        }
    }
}
