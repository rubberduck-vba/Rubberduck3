using Rubberduck.RPC.Proxy.SharedServices.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Console.Commands.Parameters;
using Rubberduck.RPC.Proxy.SharedServices.Console.Configuration;
using Rubberduck.RPC.Proxy.SharedServices.Server.Commands;
using System.Threading.Tasks;

namespace Rubberduck.RPC.Proxy.SharedServices.Console.Commands
{
    public class SetEnabledNotificationCommand : ServerNotificationCommand<ServerConsoleOptions, SetEnabledParams>
    {
        public SetEnabledNotificationCommand(IServerLogger logger, GetServerOptionsAsync<ServerConsoleOptions> getConfiguration, GetServerStateInfoAsync getServerState)
            : base(logger, getConfiguration, getServerState) { }

        public override string Description { get; } = "Pauses or resumes console output.";

        protected override async Task ExecuteInternalAsync(SetEnabledParams parameter)
        {
            var config = await GetConfigurationAsync();
            config.IsEnabled = parameter.Value;

            await Task.CompletedTask;
        }
    }
}
