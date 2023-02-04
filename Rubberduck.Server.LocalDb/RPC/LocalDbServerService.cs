using Rubberduck.RPC;
using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Proxy.LocalDbServer;
using Rubberduck.RPC.Proxy.SharedServices;
using Rubberduck.RPC.Proxy.SharedServices.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Console.Commands.Parameters;
using Rubberduck.RPC.Proxy.SharedServices.Server.Abstract;
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
    internal class LocalDbServerService : ServerService<ServerCapabilities, ILocalDbServerProxyClient>, 
        IServerProxyService<ServerCapabilities, ILocalDbServerProxyClient, ServerCommands<ServerCapabilities>>
    {
        public override ServerCommands<ServerCapabilities> Commands => throw new NotImplementedException();

        public LocalDbServerService(IServerLogger logger,
            IServerStateService<ServerCapabilities> serverStateService)
            : base(logger, serverStateService)
        {
        }

        /// <summary>
        /// Notifies the parent service that the server is ready to terminate.
        /// </summary>

        internal event EventHandler WillExit;
        private void OnWillExit() => WillExit?.Invoke(this, EventArgs.Empty);

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
                    Logger.OnInfo("All clients haver disconnected, server is shutting down.");
                }
            }
        }

        protected async void HandleExitClientNotification(object sender, EventArgs e)
        {
            OnWillExit();
            DeregisterClientNotifications(ClientProxy);
            await Commands.ExitCommand.TryExecuteAsync();
        }

        protected override void RegisterClientNotifications(ILocalDbServerProxyClient client)
        {
            client.ClientInitialized += HandleClientInitializedNotification;
            client.ClientShutdown += HandleShutdownClientNotification;
            client.RequestExit += HandleExitClientNotification;
            client.SetTrace += HandleSetTraceClientNotification;
  
            client.Initialized += Client_Initialized;
        }

        protected override void DeregisterClientNotifications(ILocalDbServerProxyClient client)
        {
            client.ClientInitialized -= HandleClientInitializedNotification;
            client.ClientShutdown -= HandleShutdownClientNotification;
            client.RequestExit -= HandleExitClientNotification;
            client.SetTrace -= HandleSetTraceClientNotification;

            client.Initialized -= Client_Initialized;
        }

        private void Client_Initialized(object sender, InitializedParams e)
        {
            throw new NotSupportedException("This server does not implement the 'initialized' LSP notification. Use 'clientInitialized' instead.");
        }
    }
}
