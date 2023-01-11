using Rubberduck.InternalApi.RPC.DataServer.Parameters;
using Rubberduck.InternalApi.RPC.DataServer.Response;
using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Rubberduck.DataServer.Controllers
{
    [ServiceContract(Name = "server")]
    public class ServerController
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
        [OperationContract(Name = "connect")]
        public async Task<ConnectResult> Connect(ConnectParams parameters)
        {
            var client = new Client
            { 
                ProcessId = parameters.ProcessId,
                Name = parameters.Name,
            };
            
            var connected = _server.Connect(client);
            
            return await Task.FromResult(new ConnectResult 
            { 
                Connected = connected 
            });
        }

        /// <summary>
        /// Disconnects a client from the server.
        /// </summary>
        /// <remarks>
        /// Server shuts down if the last client disconnects.
        /// </remarks>
        [OperationContract(Name = "disconnect")]
        public async Task Disconnect(DisconnectParams parameters)
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

            if (shuttingDown)
            {
                _environment.Exit();
            }
        }
    }
}
