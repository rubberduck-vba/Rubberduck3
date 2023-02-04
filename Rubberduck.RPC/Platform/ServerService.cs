using System.Threading;
using System.Threading.Tasks;
using Rubberduck.RPC.Platform.Exceptions;
using Rubberduck.RPC.Platform.Model;
using Rubberduck.RPC.Proxy.SharedServices;
using Rubberduck.RPC.Proxy.SharedServices.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Console.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Server.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Server.Commands;
using Rubberduck.RPC.Proxy.SharedServices.Server.Configuration;
using Rubberduck.RPC.Proxy.SharedServices.Server.Model;
using Rubberduck.RPC.Proxy.SharedServices.Telemetry;

namespace Rubberduck.RPC.Platform
{
    /// <summary>
    /// An abstract implementation of <c>IServerProxy</c>.
    /// </summary>
    /// <remarks>
    /// Proxy implementations should be stateless: the instance only lives for the duration of a single request, but an instance may be cached and reused.
    /// </remarks>
    /// <typeparam name="TOptions">The class type that defines server configuration options.</typeparam>
    public abstract class ServerService<TOptions, TServerProxyClient> : ServerProxyService<TOptions, ServerCommands<TOptions>, TServerProxyClient>, IServerProxyService<TOptions, TServerProxyClient, ServerCommands<TOptions>>
        where TOptions : SharedServerCapabilities, new()
        where TServerProxyClient : class, IServerProxyClient
    {
        /// <summary>
        /// Creates a server service to handle a RPC request or notification on the server side.
        /// </summary>
        protected ServerService(IServerLogger logger, IServerStateService<TOptions> serverStateService)
            : base(logger, serverStateService)
        {
        }

        public ServerState Info() => ServerStateService.Info;

        public IServerConsoleService<SharedServerCapabilities> ServerConsole { get; }

        public ITelemetryClientService Telemetry { get; }

        /* TODO */

        public async Task<InitializeResult<TOptions>> InitializeAsync(InitializeParams<TOptions> parameters, CancellationToken token)
        {
            if (!(await Commands.InitializeCommand.TryExecuteAsync(parameters, token)).TryOut(out var response))
            {
                throw new CommandNotExecutedException(nameof(Commands.InitializeCommand));
            }

            return response;
        }
    }
}
