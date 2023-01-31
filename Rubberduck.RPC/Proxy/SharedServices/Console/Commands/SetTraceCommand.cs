using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Proxy.SharedServices.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Console.Commands.Parameters;
using Rubberduck.RPC.Proxy.SharedServices.Console.Configuration;
using Rubberduck.RPC.Proxy.SharedServices.Server.Commands;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.RPC.Proxy.SharedServices.Console.Commands
{
    public class SetTraceCommand : ServerNotificationCommand<ServerConsoleOptions, SetTraceParams>
    {
        public SetTraceCommand(IServerLogger logger, GetServerOptions<ServerConsoleOptions> getConfiguration, GetServerStateInfo getServerState)
            : base(logger, getConfiguration, getServerState)
        {
        }

        public override string Description => "Sets the trace verbosity level of this server.";

        protected override async Task ExecuteInternalAsync(SetTraceParams parameter)
        {
            if (!Enum.TryParse<Constants.Console.VerbosityOptions.AsStringEnum>(parameter.Value, ignoreCase: true, out var newValue))
            {
                throw new ArgumentOutOfRangeException(nameof(parameter.Value));
            }

            var config = GetConfiguration();
            config.Trace = newValue;

            await Task.CompletedTask;
        }
    }
}
