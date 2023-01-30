using Rubberduck.RPC.Proxy.SharedServices.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Server.Configuration;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.RPC.Proxy.SharedServices.Server.Commands
{
    public class ExitCommand : ServerNotificationCommand<SharedServerCapabilities>
    {
        private readonly CancellationTokenSource _tokenSource;

        /// <summary>
        /// A command that terminates the server process.
        /// </summary>
        /// <param name="tokenSource">The cancellation token that controls the main server loop.</param>
        public ExitCommand(CancellationTokenSource tokenSource, IServerLogger logger, GetServerOptions<SharedServerCapabilities> getConfiguration, GetServerStateInfo getCurrentServerState) 
            : base(logger, getConfiguration, getCurrentServerState)
        {
            _tokenSource = tokenSource;
        }

        public override string Description { get; } = "Notifies any connected clients, and then terminates the server process.";

        protected override IReadOnlyCollection<ServerStatus> ExpectedServerStates => new[]
        {
            ServerStatus.Started,
            ServerStatus.AwaitingInitialized,
            ServerStatus.ShuttingDown,
        };

        protected override async Task ExecuteInternalAsync(CancellationToken token)
        {
            _tokenSource.Cancel();
            await Task.CompletedTask;
        }
    }
}
