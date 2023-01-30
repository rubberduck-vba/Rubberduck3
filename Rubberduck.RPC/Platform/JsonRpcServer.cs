using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using StreamJsonRpc;
using Rubberduck.RPC.Platform.Model;
using Rubberduck.RPC.Proxy.SharedServices;

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
    public abstract class JsonRpcServer<TStream, TServerService, TOptions, TClientProxy> 
        where TStream : Stream
        where TServerService : ServerService<TOptions, TClientProxy>
        where TOptions : class, new()
        where TClientProxy : IServerProxyClient
    {
        private readonly IRpcStreamFactory<TStream> _rpcStreamFactory;
        private readonly IServerServiceFactory<TServerService, TOptions, TClientProxy> _serviceFactory;

        private readonly BasicServerInfo _info;
        private readonly CancellationToken _token;
        private IEnumerable<Type> _proxyTypes;

        protected JsonRpcServer(
            BasicServerInfo info,
            IRpcStreamFactory<TStream> rpcStreamFactory, 
            IServerServiceFactory<TServerService, TOptions, TClientProxy> serviceFactory, 
            CancellationToken token)
        {
            _info = info;
            _rpcStreamFactory = rpcStreamFactory;
            _serviceFactory = serviceFactory;
            _token = token;
            _proxyTypes = GetProxyTypes();
        }

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
        public async Task StartAsync()
        {
            while (!_token.IsCancellationRequested)
            {
                var stream = _rpcStreamFactory.CreateNew();
                await WaitForConnectionAsync(stream, _token);
                _ = SendResponseAsync(stream, _token);
            }
            _token.ThrowIfCancellationRequested();
        }

        private TServerService _cachedService = null;
        private TServerService GetOrCreateService()
        {
            return _cachedService ?? (_cachedService = CreateServerService());
        }

        private TServerService CreateServerService()
        {

            /*
            var getServerInfoCommand = new DelegateServerRequestCommand<object, JsonRpcServerInfo>(_ => GetServerInfo.Invoke());
            var initializeCommand = new InitializeCommand();
            var setServerConfigCommand = new SetServerOptionsCommand();
            var connectCommand = new ConnectClientCommand();
            var disconnectCommand = new DisconnectClientCommand();
            var exitCommand = new ExitCommand(console, GetServerInfo);

            var commands = new ServerCommands<TOptions>(getServerInfoCommand, initializeCommand, setServerConfigCommand, connectCommand, disconnectCommand, exitCommand);
            */
            return _serviceFactory.Create(null);
        }

        private async Task SendResponseAsync(Stream stream, CancellationToken token)
        {
            using (var rpc = new JsonRpc(stream))
            {
                foreach (var type in _proxyTypes)
                {
                    rpc.AddLocalRpcTarget(type);
                }
                token.ThrowIfCancellationRequested();

                var service = GetOrCreateService();
                rpc.AddLocalRpcTarget(service, new JsonRpcTargetOptions
                {
                    MethodNameTransform = MethodNameTransform,
                    EventNameTransform = EventNameTransform,
                    AllowNonPublicInvocation = false,
                    ClientRequiresNamedArguments = true,
                    DisposeOnDisconnect = true,
                    NotifyClientOfEvents = true,
                    UseSingleObjectParameterDeserialization = true,
                });

                rpc.StartListening();
                await rpc.Completion;
            }
        }

        private string MethodNameTransform(string name)
        {
            var camelCased = CommonMethodNameTransforms.CamelCase(name);
            return camelCased;
        }

        private string EventNameTransform(string name)
        {
            var camelCased = CommonMethodNameTransforms.CamelCase(name);
            return camelCased;
        }
    }
}
