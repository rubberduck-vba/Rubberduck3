using Rubberduck.RPC.Proxy.SharedServices.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Server.Configuration;
using Rubberduck.RPC.Proxy.SharedServices.Server.Responses;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.RPC.Proxy.SharedServices.Server.Commands
{
    public class ConnectClientCommand : ServerRequestCommand<ClientInfo, ConnectResult, SharedServerCapabilities>
    {
        public ConnectClientCommand(IServerLogger logger, GetServerOptions<SharedServerCapabilities> getConfiguration, GetServerStateInfo getServerState) 
            : base(logger, getConfiguration, getServerState)
        {
        }

        public override string Description { get; } = "Connects a client process to a server.";

        protected override async Task<ConnectResult> ExecuteInternalAsync(ClientInfo parameter, CancellationToken token)
        {
            var info = ValidateParameter(parameter);
            token.ThrowIfCancellationRequested();

            var state = GetCurrentServerStateInfo.Invoke();
            token.ThrowIfCancellationRequested();

            var isConnected = state.Connect(info);
            Logger.OnInfo($"Client '{info.Name}' (ProcessID: {info.ProcessId}) has connected.");

            return await Task.FromResult(new ConnectResult { Connected = isConnected });
        }

        private ClientInfo ValidateParameter(ClientInfo parameter)
        {
            var info = parameter ?? throw new ArgumentNullException(nameof(parameter), "Parameter cannot be null.");

            if (info.ProcessId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(info.ProcessId), info.ProcessId, "An invalid process ID was supplied.");
            }

            if (string.IsNullOrWhiteSpace(info.Name))
            {
                throw new ArgumentNullException(nameof(info.Name), "Client name cannot be null or empty/whitespace.");
            }

            if (string.IsNullOrWhiteSpace(info.Version))
            {
                throw new ArgumentNullException(nameof(info.Version), "Client version cannot be null or empty/whitespace.");
            }

            return info;
        }
    }
}
