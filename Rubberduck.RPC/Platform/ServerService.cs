using System.Threading;
using System.Threading.Tasks;
using Rubberduck.RPC.Platform.Exceptions;
using Rubberduck.RPC.Platform.Model;
using Rubberduck.RPC.Proxy.SharedServices;
using Rubberduck.RPC.Proxy.SharedServices.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Console.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Console.Configuration;
using Rubberduck.RPC.Proxy.SharedServices.Server.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Server.Commands;
using Rubberduck.RPC.Proxy.SharedServices.Server.Model;

namespace Rubberduck.RPC.Platform
{
    /// <summary>
    /// An abstract implementation of <c>IServerProxy</c>.
    /// </summary>
    /// <remarks>
    /// Proxy implementations should be stateless: the instance only lives for the duration of a single request, but an instance may be cached and reused.
    /// </remarks>
    /// <typeparam name="TOptions">The class type that defines server configuration options.</typeparam>
    public abstract class ServerService<TOptions, TClientProxy> : ServerProxyService<TOptions, ServerCommands<TOptions>>, IServerProxyService<TOptions>
        where TOptions : class, new()
        where TClientProxy : IServerProxyClient
    {
        /// <summary>
        /// Creates a server service to handle a RPC request or notification on the server side.
        /// </summary>
        protected ServerService(IServerLogger logger, TClientProxy clientProxy, GetServerOptions<TOptions> getConfiguration, GetServerStateInfo getServerState)
            : base(logger, getConfiguration, getServerState)
        {
            ClientProxy = clientProxy;
            RegisterNotifications(clientProxy);
        }

        public ServerState Info()
        {
            return ServerState;
        }

        /// <summary>
        /// Represents a client-side notifications provider.
        /// </summary>
        protected TClientProxy ClientProxy { get; }

        public IServerConsoleService<ServerConsoleOptions> ServerConsole { get; }

        /* TODO */


        public async Task<InitializeResult<TOptions>> InitializeAsync(InitializeParams<TOptions> parameters, CancellationToken token)
        {
            if (!(await Commands.InitializeCommand.TryExecuteAsync(parameters, token)).TryOut(out var response))
            {
                throw new CommandNotExecutedException(nameof(Commands.InitializeCommand));
            }

            return response;
        }

        /// <summary>
        /// Registers client event/notification handlers for the specified client proxy.
        /// </summary>
        /// <param name="proxy">The notification provider proxy.</param>
        /// <remarks>
        /// Event handlers should invoke the corresponding command, as appropriate.
        /// </remarks>
        protected abstract void RegisterNotifications(TClientProxy proxy);
        /// <summary>
        /// Registers client event/notification handlers for the specified client proxy.
        /// </summary>
        /// <param name="proxy">The notification provider proxy.</param>
        protected abstract void DeregisterNotifications(TClientProxy proxy);
    }
}
