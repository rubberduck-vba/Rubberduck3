using System;
using System.Threading;
using System.Threading.Tasks;
using Rubberduck.RPC.Platform.Exceptions;
using Rubberduck.RPC.Platform.Model;
using Rubberduck.RPC.Proxy.SharedServices;
using Rubberduck.RPC.Proxy.SharedServices.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Server.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Server.Commands;
using Rubberduck.RPC.Proxy.SharedServices.Server.Configuration;
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
    public abstract class ServerService<TOptions, TInitializeParams> : ServerSideProxyService<TOptions>, IServerProxy<TOptions, TInitializeParams>
        where TOptions : SharedServerCapabilities, new()
        where TInitializeParams : class, new()
    {
        /// <summary>
        /// Creates a server service to handle a RPC request or notification on the server side.
        /// </summary>
        protected ServerService(IServerLogger logger, IServerStateService<TOptions> serverStateService)
            : base(logger, serverStateService)
        {
        }

        public event EventHandler WillExit;
        public async Task OnWillExitAsync() => await Task.Run(() => WillExit?.Invoke(this, EventArgs.Empty));

        public async Task<ServerState> RequestServerInfoAsync() => await Task.FromResult(ServerStateService.Info);

        private readonly InitializeCommand<TOptions, TInitializeParams> _initializeCommand;
        public async Task<InitializeResult<TOptions>> InitializeAsync(TInitializeParams parameters, CancellationToken token)
        {
            if (!(await _initializeCommand.TryExecuteAsync(parameters, token)).TryOut(out var response))
            {
                throw new CommandNotExecutedException(typeof(InitializeCommand<TOptions, TInitializeParams>).Name);
            }

            return response;
        }

    }
}
