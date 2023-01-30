using Rubberduck.RPC.Platform.Model;
using Rubberduck.RPC.Proxy.SharedServices.Abstract;

namespace Rubberduck.RPC.Proxy.SharedServices.Server.Commands
{
    public delegate ServerState GetServerStateInfo();
    public delegate TOptions GetServerOptions<out TOptions>() where TOptions : class, new();

    /// <summary>
    /// Represents a server command that abstracts away its execution.
    /// </summary>
    /// <typeparam name="TOptions">The class type of the configuration options for the service that owns this command.</typeparam>
    public abstract class ServerCommandBase<TOptions>
        where TOptions : class, new()
    {
        protected ServerCommandBase(IServerLogger logger, GetServerOptions<TOptions> getConfiguration, GetServerStateInfo getCurrentServerState)
        {
            Logger = logger;
            GetCurrentServerStateInfo = getCurrentServerState;
            GetConfiguration = getConfiguration;
        }

        /// <summary>
        /// Outputs log messages to the console.
        /// </summary>
        protected IServerLogger Logger { get; }
        /// <summary>
        /// Gets the current server state information.
        /// </summary>
        protected GetServerStateInfo GetCurrentServerStateInfo { get; }
        /// <summary>
        /// Gets the current configuration options for the service that owns this command.
        /// </summary>
        protected GetServerOptions<TOptions> GetConfiguration { get; }

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
