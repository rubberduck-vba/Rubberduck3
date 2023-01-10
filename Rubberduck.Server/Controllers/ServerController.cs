using Rubberduck.InternalApi.RPC;
using Rubberduck.InternalApi.RPC.LSP.Parameters;
using Rubberduck.InternalApi.RPC.LSP.Response;
using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Rubberduck.Server.Controllers
{
    [ServiceContract]
    public class ServerController
    {
        [OperationContract(Name = "initialize")]
        public async Task<InitializeResult> Initialize(InitializeParams parameters)
        {
            return null;
        }

        [OperationContract(Name = "initialized")]
        public async Task Initialized(InitializedParams parameters)
        {
        }

        [OperationContract(Name = "shutdown")]
        public async Task Shutdown()
        {
        }

        [OperationContract(Name = "exit")]
        public async Task Exit()
        {
            // TODO exit with code 1 if any requests were received between the Shutdown() and the Exit() calls.
            Environment.Exit(0);
        }

        [OperationContract(Name = "$/setTrace")]
        public async Task SetTrace(SetTraceParams parameters)
        {
        }

        [OperationContract(Name = "$/logTrace")]
        public async Task LogTrace(LogTraceParams parameters)
        {
        }
    }
}
