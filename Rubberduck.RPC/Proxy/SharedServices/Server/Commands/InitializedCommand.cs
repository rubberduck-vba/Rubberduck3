using Rubberduck.RPC.Proxy.SharedServices.Abstract;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.RPC.Proxy.SharedServices.Server.Commands
{
    public class InitializedCommand<TOptions> : ServerNotificationCommand<TOptions, InitializedParams>
        where TOptions : class, new()
    {
        public InitializedCommand(IServerLogger logger, GetServerOptions<TOptions> getConfiguration, GetServerStateInfo getServerState) 
            : base(logger, getConfiguration, getServerState)
        {
        }

        public override string Description { get; } = "A client-to-server notification that concludes the client/server initialization process.";

        protected override IReadOnlyCollection<ServerStatus> ExpectedServerStates => new[]
        {
            ServerStatus.AwaitingInitialized,
        };

        protected override async Task ExecuteInternalAsync(InitializedParams parameter)
        {
            var state = GetCurrentServerStateInfo.Invoke();
            state.Status = ServerStatus.Initialized;

            Logger.OnInfo("Initialization completed.", $"Clients connected: {state.ClientsCount}.");
            await Task.CompletedTask;
        }
    }
}
