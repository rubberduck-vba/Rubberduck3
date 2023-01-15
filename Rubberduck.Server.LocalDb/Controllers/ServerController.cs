using AustinHarris.JsonRpc;
using NLog;
using Rubberduck.InternalApi.RPC;
using Rubberduck.InternalApi.RPC.DataServer;
using Rubberduck.InternalApi.RPC.DataServer.Parameters;
using Rubberduck.InternalApi.RPC.DataServer.Response;
using Rubberduck.RPC.Proxy;
using Rubberduck.RPC.Proxy.Controllers;
using System;

namespace Rubberduck.Server.LocalDb.Controllers
{
    public class ServerController : JsonRpcService, ILocalDbServerController
    {
        private readonly IEnvironmentService _environment;
        private readonly LocalDbServer _server;

        internal ServerController(IEnvironmentService environment, LocalDbServer server)
        {
            _environment = environment;
            _server = server;
        }

        /// <summary>
        /// Connects a client to the server.
        /// </summary>
        [JsonRpcMethod(JsonRpcMethods.Connect)]
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

        private bool _awaitingExit = false;

        /// <summary>
        /// Disconnects a client from the server.
        /// </summary>
        [JsonRpcMethod(JsonRpcMethods.Disconnect)]
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

            _awaitingExit = shuttingDown;

            return new DisconnectResult
            {
                Disconnected = disconnected,
                ShuttingDown = shuttingDown,
            };
        }

        /// <summary>
        /// Terminates the server process.
        /// </summary>
        [JsonRpcMethod(JsonRpcMethods.Exit)]
        public void Exit()
        {
            var exitCode = 0;
            if (!_awaitingExit)
            {
                _server.Console.Log(LogLevel.Warn, "Unexpected exit notification.", verbose: "Exiting with code 1.");
                exitCode = 1;
            }

            _environment.Exit(exitCode);
        }
    }
}
