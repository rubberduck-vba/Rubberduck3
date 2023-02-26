using Microsoft.Extensions.Logging;
using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Model;
using Rubberduck.RPC.Platform.Model.LocalDb.Responses;
using System.Threading.Tasks;

namespace Rubberduck.Server.LocalDb.RPC.Info
{
    internal class InfoHandler : JsonRpcRequestHandler<InfoRequest, InfoResult>
    {
        private readonly ServerState _serverState;

        public InfoHandler(ILogger logger, ServerState serverState)
            : base(logger)
        {
            _serverState = serverState;
        }

        protected override async Task<InfoResult> HandleAsync(InfoRequest request)
        {
            var state = new InfoResult
            {
                Clients = _serverState.Clients,
                GC = _serverState.GC,
                IsAlive= _serverState.IsAlive,
                MessagesReceived= _serverState.MessagesReceived,
                MessagesSent= _serverState.MessagesSent,
                Name= _serverState.Name,
                PeakWorkingSet= _serverState.PeakWorkingSet,
                ProcessId= _serverState.ProcessId,
                StartTime= _serverState.StartTime,
                Status= _serverState.Status,
                Threads= _serverState.Threads,
                Version= _serverState.Version,
                WorkingSet = _serverState.WorkingSet
            };

            return await Task.FromResult(state);
        }
    }
}
