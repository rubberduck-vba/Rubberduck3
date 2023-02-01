using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using StreamJsonRpc;
using Rubberduck.RPC.Platform.Model;

namespace Rubberduck.RPC.Platform
{
    /// <summary>
    /// A marker interface for a type to register as a JsonRpc target.
    /// </summary>
    /// <remarks>
    /// Might turn into an attribute
    /// </remarks>
    public interface IJsonRpcTarget { }

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
        Task RunAsync(CancellationToken token);
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
        private readonly IEnumerable<IJsonRpcTarget> _proxies;

        protected JsonRpcServer(IRpcStreamFactory<TStream> rpcStreamFactory, IEnumerable<IJsonRpcTarget> proxies)
        {
            _rpcStreamFactory = rpcStreamFactory ?? throw new ArgumentNullException(nameof(rpcStreamFactory));
            _proxies = proxies ?? throw new ArgumentNullException(nameof(proxies));
        }

        public ServerState Info => null; // _getServerState.Invoke();

        /// <summary>
        /// Wait for a client to connect.
        /// </summary>
        protected abstract Task WaitForConnectionAsync(TStream stream, CancellationToken token);


        /// <summary>
        /// Starts the server.
        /// </summary>
        /// <remarks>
        /// Creates a new RPC stream for each new client connection, until cancellation is requested on the token.
        /// </remarks>
        public async Task RunAsync(CancellationToken serverToken)
        {
            Console.WriteLine("Registered RPC server targets:");
            foreach (var proxy in _proxies)
            {
                Console.WriteLine($" * {proxy.GetType().Name}");
            }

            Console.WriteLine("Server started. Awaiting client connection...");
            
            while (!serverToken.IsCancellationRequested)
            {
                // our stream only buffers a single message, so we need a new one every time:
                var stream = _rpcStreamFactory.CreateNew();
                await WaitForConnectionAsync(stream, serverToken);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

                // we specifically *do NOT want* to wait for the response here.
                // awaiting this call would block the thread and prevent handling other incoming requests while a response is cooking.

                /*await*/ SendResponseAsync(stream, serverToken); 

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }

            Console.WriteLine("Server has stopped.");
            serverToken.ThrowIfCancellationRequested();
        }

        private static readonly JsonRpcTargetOptions _targetOptions = new JsonRpcTargetOptions
        {
            MethodNameTransform = MethodNameTransform,
            EventNameTransform = EventNameTransform,
            AllowNonPublicInvocation = false,
            ClientRequiresNamedArguments = true,
            DisposeOnDisconnect = true,
            NotifyClientOfEvents = true,
            UseSingleObjectParameterDeserialization = true,
        };

    private async Task SendResponseAsync(Stream stream, CancellationToken token)
        {
            using (var rpc = new JsonRpc(stream))
            {
                foreach (var proxy in _proxies)
                {
                    rpc.AddLocalRpcTarget(proxy, _targetOptions);
                }

                token.ThrowIfCancellationRequested();

                rpc.StartListening();
                await rpc.Completion;
            }
        }

        private static string EventNameTransform(string name)
        {
            if (RpcMethodNameMappings.IsMappedEvent(name, out var mapped))
            {
                return mapped;
            }

            System.Diagnostics.Debug.WriteLine($"Event '{name}' is not mapped to an explicit RPC method name.");
            var camelCased = CommonMethodNameTransforms.CamelCase(name);
            return camelCased;
        }

        private static string MethodNameTransform(string name)
        {
            if (RpcMethodNameMappings.IsMappedEvent(name, out var mapped))
            {
                return mapped;
            }

            System.Diagnostics.Debug.WriteLine($"Event '{name}' is not mapped to an explicit RPC method name.");
            var camelCased = CommonMethodNameTransforms.CamelCase(name);
            return camelCased;
        }
    }
}
