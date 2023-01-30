using Rubberduck.RPC.Proxy.SharedServices.Console.Commands;

namespace Rubberduck.RPC.Proxy.SharedServices
{
    public interface IConsoleCommandsService
    {
        /// <summary>
        /// Exposes console commands.
        /// </summary>
        ServerConsoleCommands Commands { get; }
    }
}