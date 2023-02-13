using Rubberduck.RPC.Proxy.SharedServices.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Console.Configuration;
using Rubberduck.RPC.Proxy.SharedServices.Server.Commands;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.RPC.Proxy.SharedServices.Console.Commands
{
    public class GetConsoleOptionsCommand : ServerRequestCommand<ServerConsoleOptions, ServerConsoleOptions>
    {
        public GetConsoleOptionsCommand(IServerLogger logger, GetServerOptionsAsync<ServerConsoleOptions> getConfiguration, GetServerStateInfoAsync getServerState) 
            : base(logger, getConfiguration, getServerState) { }

        public override string Description { get; } = "Gets the current server console configuration.";

        protected override async Task<ServerConsoleOptions> ExecuteInternalAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            return await GetConfigurationAsync();
        }
    }
}
