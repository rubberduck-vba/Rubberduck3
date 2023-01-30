using Rubberduck.RPC.Proxy.SharedServices.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Server.Configuration;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.RPC.Proxy.SharedServices.Server.Commands
{
    public class ShutdownCommand : ServerVoidRequestCommand<SharedServerCapabilities>
    {
        private readonly Action _shutdownAction;

        public ShutdownCommand(Action shutdownAction, IServerLogger logger, GetServerOptions<SharedServerCapabilities> getConfiguration, GetServerStateInfo getServerState)
            : base(logger, getConfiguration, getServerState)
        {
            _shutdownAction = shutdownAction;
        }

        public override string Description { get; } = "Shuts down the server without exiting the process and awaits an 'Exit' notification.";

        protected sealed override IReadOnlyCollection<ServerStatus> ExpectedServerStates => new[]
        {
            ServerStatus.AwaitingInitialized,
            ServerStatus.Initialized,
        };

        protected override async Task ExecuteInternalAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            await Task.Run(_shutdownAction, token);
        }
    }

    public abstract class ShutdownCommand<TParameter> : ServerVoidRequestCommand<TParameter, SharedServerCapabilities>
        where TParameter : class, new()
    {
        private readonly Action<TParameter> _shutdownAction;

        public ShutdownCommand(Action<TParameter> shutdownAction, IServerLogger logger, GetServerOptions<SharedServerCapabilities> getConfiguration, GetServerStateInfo getServerState)
            : base(logger, getConfiguration, getServerState)
        {
            _shutdownAction = shutdownAction;
        }

        protected override async Task ExecuteInternalAsync(TParameter parameters, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            await Task.Run(() => _shutdownAction.Invoke(parameters), token);
        }

        protected sealed override IReadOnlyCollection<ServerStatus> ExpectedServerStates => new[]
        {
            ServerStatus.AwaitingInitialized,
            ServerStatus.Initialized,
        };
    }
}
