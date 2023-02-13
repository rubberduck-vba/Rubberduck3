using Rubberduck.RPC.Platform.Model;
using Rubberduck.RPC.Proxy.SharedServices.Abstract;
using System.Threading.Tasks;

namespace Rubberduck.RPC.Proxy.SharedServices.Server.Commands
{
    public delegate Task<ServerState> GetServerStateInfoAsync();
    public delegate Task<TOptions> GetServerOptionsAsync<TOptions>() where TOptions : class, new();

    /// <summary>
    /// Represents a server command that abstracts away its execution.
    /// </summary>
    /// <typeparam name="TOptions">The class type of the configuration options for the service that owns this command.</typeparam>
    public abstract class ServerCommandBase<TOptions>
        where TOptions : class, new()
    {
        protected ServerCommandBase(IServerLogger logger, GetServerOptionsAsync<TOptions> getConfiguration, GetServerStateInfoAsync getCurrentServerState)
        {
            Logger = logger;
            GetCurrentServerStateInfoAsync = getCurrentServerState;
            GetConfigurationAsync = getConfiguration;
        }

        /// <summary>
        /// Outputs log messages to the console.
        /// </summary>
        protected IServerLogger Logger { get; }
        /// <summary>
        /// Gets the current server state information.
        /// </summary>
        protected GetServerStateInfoAsync GetCurrentServerStateInfoAsync { get; }
        /// <summary>
        /// Gets the current configuration options for the service that owns this command.
        /// </summary>
        protected GetServerOptionsAsync<TOptions> GetConfigurationAsync { get; }

        /// <summary>
        /// The name of this command.
        /// </summary>
        /// <remarks>
        /// Returns the type name unless overridden.
        /// </remarks>
        public virtual string Name => GetType().Name;
        /// <summary>
        /// A short description of what this command does.
        /// </summary>
        public abstract string Description { get; }
    }
}
