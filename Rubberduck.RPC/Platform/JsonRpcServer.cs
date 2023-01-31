using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using StreamJsonRpc;
using Rubberduck.RPC.Platform.Model;
using Rubberduck.RPC.Proxy.SharedServices;
using Rubberduck.RPC.Proxy.SharedServices.Server.Commands;
using System.Linq;
using System.Reflection;
using Rubberduck.RPC.Platform.Metadata;

namespace Rubberduck.RPC.Platform
{
    /// <summary>
    /// Represents a JsonRPC server.
    /// </summary>
    public interface IJsonRpcServer
    {
        /// <summary>
        /// Gets information about this server instance.
        /// </summary>
        ServerState Info { get; }

        /// <summary>
        /// Starts the RPC server.
        /// </summary>
        Task StartAsync(CancellationToken token);
    }

    /// <summary>
    /// Represents a server process.
    /// </summary>
    /// <remarks>
    /// Implementation holds the server state for the lifetime of the host process.
    /// </remarks>
    public abstract class JsonRpcServer<TStream, TServerService, TOptions> : IJsonRpcServer
        where TStream : Stream
        where TServerService : ServerService<TOptions>
        where TOptions : class, new()
    {
        private readonly IRpcStreamFactory<TStream> _rpcStreamFactory;
        private readonly IServerServiceFactory<TServerService, TOptions> _serviceFactory;

        private readonly GetServerStateInfo _getServerState;
        private readonly BasicServerInfo _info;
        private IEnumerable<Type> _proxyTypes;

        protected JsonRpcServer(BasicServerInfo info, GetServerStateInfo getServerState,
            IRpcStreamFactory<TStream> rpcStreamFactory, 
            IServerServiceFactory<TServerService, TOptions> serviceFactory)
        {
            _info = info;
            _getServerState = getServerState;
            _rpcStreamFactory = rpcStreamFactory;
            _serviceFactory = serviceFactory;
            _proxyTypes = GetProxyTypes();
        }

        public ServerState Info => _getServerState.Invoke();

        /// <summary>
        /// Wait for a client to connect.
        /// </summary>
        protected abstract Task WaitForConnectionAsync(TStream stream, CancellationToken token);

        /// <summary>
        /// Gets all proxy types to register for this server.
        /// </summary>
        protected abstract IEnumerable<Type> GetProxyTypes();

        /// <summary>
        /// Starts the server.
        /// </summary>
        /// <remarks>
        /// Creates a new RPC stream for each new client connection, until cancellation is requested on the token.
        /// </remarks>
        public async Task StartAsync(CancellationToken serverToken)
        {
            Console.WriteLine("Server started. Awaiting client connection...");

            while (!serverToken.IsCancellationRequested)
            {
                var stream = _rpcStreamFactory.CreateNew();
                await WaitForConnectionAsync(stream, serverToken);
                _ = SendResponseAsync(stream, serverToken);
            }
            serverToken.ThrowIfCancellationRequested();
        }

        private async Task SendResponseAsync(Stream stream, CancellationToken token)
        {
            using (var rpc = new JsonRpc(stream))
            {
                var targetOptions = new JsonRpcTargetOptions
                {
                    EventNameTransform = EventNameTransform,
                    AllowNonPublicInvocation = false,
                    ClientRequiresNamedArguments = true,
                    DisposeOnDisconnect = true,
                    NotifyClientOfEvents = true,
                    UseSingleObjectParameterDeserialization = true,
                };

                foreach (var type in _proxyTypes)
                {
                    rpc.AddLocalRpcTarget(type, targetOptions);
                }

                token.ThrowIfCancellationRequested();

                rpc.StartListening();
                await rpc.Completion;
            }
        }

        private static readonly IDictionary<string, string> _mappedEventNames = (from e in typeof(IJsonRpcServer).Assembly.GetTypes().SelectMany(t => t.GetEvents())
                                                                                 let lsp = e.GetCustomAttribute<RubberduckSPAttribute>(inherit:true)
                                                                                 where lsp != null
                                                                                 select (EventName: e.Name, RpcMethodName: lsp.MethodName)).ToDictionary(e => e.EventName, e => e.RpcMethodName);

        private string EventNameTransform(string name)
        {
            if (_mappedEventNames.TryGetValue(name, out var mapped))
            {
                return mapped;
            }

            System.Diagnostics.Debug.WriteLine($"Event '{name}' is not mapped to an explicit RPC method name.");
            var camelCased = CommonMethodNameTransforms.CamelCase(name);
            return camelCased;
        }
    }
}
