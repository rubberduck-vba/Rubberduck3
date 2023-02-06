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
    /// <summary>
    /// A command that is executed in responser to a '<c>$/setTrace</c>' notification sent from a client.
    /// </summary>
    public class SetTraceNotificationCommand : ServerNotificationCommand<ServerConsoleOptions, SetTraceParams>
    {
        public SetTraceNotificationCommand(IServerLogger logger, GetServerOptions<ServerConsoleOptions> getConfiguration, GetServerStateInfo getServerState)
            : base(logger, getConfiguration, getServerState)
        {
        }

        public override string Description => "Sets the trace verbosity level of this server.";

        protected override async Task ExecuteInternalAsync(SetTraceParams parameter)
        {
            var config = GetConfiguration();
            config.Trace = parameter.Value;

            await Task.CompletedTask;
        }
    }
}
