using Rubberduck.RPC.Proxy.SharedServices.Server.Commands;

namespace Rubberduck.RPC.Proxy.SharedServices.Server.Abstract
{
    public interface IServerCommandsProxy<TOptions>
        where TOptions : class, new()
    {
        /// <summary>
        /// Exposes server commands.
        /// </summary>
        ServerCommands<TOptions> Commands { get; }
    }
}