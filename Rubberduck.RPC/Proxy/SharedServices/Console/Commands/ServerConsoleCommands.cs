using Rubberduck.RPC.Proxy.SharedServices.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Console.Commands.Parameters;
using Rubberduck.RPC.Proxy.SharedServices.Console.Configuration;

namespace Rubberduck.RPC.Proxy.SharedServices.Console.Commands
{
    /// <summary>
    /// The server-side console commands.
    /// </summary>
    /// <remarks>
    /// Command execution should trace to an incoming RPC method call.
    /// </remarks>
    public class ServerConsoleCommands
    {
        public ServerConsoleCommands(
            IServerNotificationCommand<SetTraceParams> setTrace,
            IServerNotificationCommand<SetEnabledParams> setEnabled,
            IServerRequestCommand<ServerConsoleOptions> getOptions) 
        {
            SetTraceCommand = setTrace;
            SetEnabledCommand = setEnabled;
            GetConsoleOptionsCommand = getOptions;
        }

        /// <summary>
        /// A server command that responds to a client notification.
        /// </summary>
        public IServerNotificationCommand<SetTraceParams> SetTraceCommand { get; }

        /// <summary>
        /// A server command that responds to a client notification by modifying console settings.
        /// </summary>
        public IServerNotificationCommand<SetEnabledParams> SetEnabledCommand { get; }

        /// <summary>
        /// A server command that response to a client request with the current server console settings.
        /// </summary>
        /// <remarks>
        /// This command is available in the following server states:
        /// <list type="bullet">
        /// <item><see cref="ServerStatus.Initialized"/></item>
        /// </list>
        /// </remarks>
        public IServerRequestCommand<ServerConsoleOptions> GetConsoleOptionsCommand { get; }
    }
}
