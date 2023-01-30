using Rubberduck.RPC.Platform.Model;
using Rubberduck.RPC.Proxy.SharedServices.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Server.Commands;

namespace Rubberduck.RPC.Proxy.SharedServices.Server.Abstract
{
    /// <summary>
    /// A common base class for all server proxy services that expose the server-side RPC targets.
    /// </summary>
    /// <typeparam name="TOptions">The class type of the configuration options for this service.</typeparam>
    /// <typeparam name="TCommands">The class type of the class that exposes the commands for this service.</typeparam>
    public abstract class ServerProxyService<TOptions, TCommands> : IConfigurableProxy<TOptions>
        where TOptions : class, new()
        where TCommands : class
    {
        private readonly GetServerOptions<TOptions> _getConfiguration;
        private readonly GetServerStateInfo _getServerState;

        protected ServerProxyService(IServerLogger logger, GetServerOptions<TOptions> getConfiguration, GetServerStateInfo getServerState)
        {
            Logger = logger;
            _getConfiguration = getConfiguration;
            _getServerState = getServerState;
        }

        public abstract TCommands Commands { get; }
        public virtual IServerLogger Logger { get; }

        public TOptions Configuration => _getConfiguration.Invoke();
        /// <summary>
        /// Gets the current server state.
        /// </summary>
        public ServerState ServerState => _getServerState.Invoke();
    }
}
