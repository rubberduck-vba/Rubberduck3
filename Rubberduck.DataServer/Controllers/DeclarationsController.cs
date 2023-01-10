using Rubberduck.InternalApi.RPC.DataServer.Parameters;
using Rubberduck.InternalApi.RPC.DataServer.Response;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Rubberduck.DataServer.Controllers
{
    [ServiceContract(Name = "server")]
    public class ServerController
    {
        private readonly Server _server;

        internal ServerController(Server server)
        {
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
        [OperationContract(Name = "disconnect")]
        public async Task<DisconnectResult> Disconnect(DisconnectParams parameters)
        {
            var client = new Client
            {
                ProcessId = parameters.ProcessId,
            };
            
            var disconnected = _server.Disconnect(client);
            var shuttingDown = !_server.HasClients;

            return new DisconnectResult
            {
                Disconnected = disconnected,
                ShuttingDown = shuttingDown,
            };
        }
    }

    [ServiceContract]
    public class DeclarationsController
    {

    }
}
