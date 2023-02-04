using Rubberduck.RPC;
using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Proxy.SharedServices;
using Rubberduck.RPC.Proxy.SharedServices.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Console.Commands.Parameters;
using Rubberduck.RPC.Proxy.SharedServices.Server.Commands;
using System;
using System.Threading;

namespace Rubberduck.Server.LocalDb.Services
{
    /// <summary>
    /// A proxy service that implements <c>IServerProxy</c>.
    /// </summary>
    /// <remarks>
    /// Proxies should be stateless: the instance may be request-scoped.
    /// </remarks>
    internal class LocalDbServerService : ServerService<ServerCapabilities>
    {
        public LocalDbServerService(IServerLogger logger,
            IServerStateService<ServerCapabilities> serverStateService)
            : base(null, logger, serverStateService)
        {
        }

        protected override void RegisterNotifications(IServerProxyClient client)
        {
            //client.ClientInitialized += HandleClientInitializedNotification;
            //client.ClientShutdown += HandleShutdownClientNotification;
            //client.RequestExit += HandleExitClientNotification;
        }

        protected override void DeregisterNotifications(IServerProxyClient client)
        {
            //client.ClientInitialized -= HandleClientInitializedNotification;
            //client.ClientShutdown -= HandleShutdownClientNotification;
            //client.RequestExit -= HandleExitClientNotification;
        }

        public override ServerCommands<ServerCapabilities> Commands { get; }

        /// <summary>
        /// Notifies the parent service that the server is ready to terminate.
        /// </summary>

        internal event EventHandler WillExit;
        private void OnWillExit() => WillExit?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Notifies the parent service that a client has connected.
        /// </summary>
        internal event EventHandler<ClientInfo> ClientConnected;
        private void OnClientConnected(ClientInfo info) => ClientConnected?.Invoke(this, info);

        /// <summary>
        /// Notifies the parent service that a client has disconnected.
        /// </summary>
        internal event EventHandler<ClientInfo> ClientDisconnected;
        private void OnClientDisconnected(ClientInfo info) => ClientDisconnected?.Invoke(this, info);

        public void HandleClientInitializedNotification(object sender, ClientInitializedParams e)
        {
            try
            {
                ServerStateService.Info.SetInitialized(e.ProcessId);
                Logger.OnInfo("Client initialized.", $"Process ID: {e.ProcessId}.");
            }
            catch (Exception exception)
            { 
                Logger.OnError(exception);
            }
        }

        protected async void HandleSetTraceClientNotification(object sender, SetTraceParams e)
        {
            if (await ServerConsole.Commands.SetTraceCommand.TryExecuteAsync(e))
            {
                Logger.OnInfo("Trace configuration set.", $"Value: {e.Value}");
            }
            else
            {
                Logger.OnWarning("Trace configuration was not set.", $"Value: {e.Value}");
            }
        }

        protected async void HandleShutdownClientNotification(object sender, ClientShutdownParams e)
        {
            if ((await Commands.DisconnectClientCommand.TryExecuteAsync(new ClientShutdownParams { ProcessId = e.ProcessId }, CancellationToken.None)).TryOut(out var result))
            {
                if (result.ShuttingDown)
                {

                }
            }
        }

        protected async void HandleExitClientNotification(object sender, EventArgs e)
        {
            OnWillExit();
            DeregisterNotifications(ClientProxy);
            await Commands.ExitCommand.TryExecuteAsync();
        }

        protected override void RpcClientSetTrace(object sender, SetTraceParams e)
        {
            throw new NotImplementedException();
        }

        protected override void RpcClientRequestExit(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        protected override void RpcClientInitialized(object sender, InitializedParams e)
        {
            throw new NotImplementedException();
        }
    }
}
