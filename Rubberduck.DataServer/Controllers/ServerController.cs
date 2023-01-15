using System;
using Rubberduck.InternalApi.RPC.DataServer.Parameters;
using Rubberduck.InternalApi.RPC.DataServer.Response;
using Rubberduck.DataServer.Services;
using AustinHarris.JsonRpc;

namespace Rubberduck.DataServer.Controllers
{
    public interface IServerController
    {
        ConnectResult Connect(ConnectParams parameters);
        DisconnectResult Disconnect(DisconnectParams parameters);
    }

    public class ServerController : JsonRpcService, IServerController
    {
        private readonly IEnvironmentService _environment;
        private readonly Server _server;

        internal ServerController(IEnvironmentService environment, Server server)
        {
            _environment = environment;
            _server = server;
        }

        /// <summary>
        /// Connects a client to the server.
        /// </summary>
        [JsonRpcMethod("connect")]
        public ConnectResult Connect(ConnectParams parameters)
        {
            var client = new Client
            { 
                ProcessId = parameters.ProcessId,
                Name = parameters.Name,
            };
            
            var connected = _server.Connect(client);
            
            return new ConnectResult 
            { 
                Connected = connected 
            };
        }

        /// <summary>
        /// Disconnects a client from the server.
        /// </summary>
        /// <remarks>
        /// Server shuts down if the last client disconnects.
        /// </remarks>
        [JsonRpcMethod("disconnect")]
        public DisconnectResult Disconnect(DisconnectParams parameters)
        {
            if (!_server.HasClients)
            {
                throw new InvalidOperationException("Server has no connected client.");
            }

            var client = new Client
            {
                ProcessId = parameters.ProcessId,
            };
            
            var disconnected = _server.Disconnect(client);
            var shuttingDown = disconnected && !_server.HasClients;

            // TODO actually shutdown if we're supposed to
            return new DisconnectResult
            {
                Disconnected = disconnected,
                ShuttingDown = shuttingDown,
            };
        }
    }
}
