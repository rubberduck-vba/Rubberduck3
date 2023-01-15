using AustinHarris.JsonRpc;
using Rubberduck.InternalApi.RPC.LSP;
using Rubberduck.InternalApi.RPC.LSP.Parameters;
using Rubberduck.InternalApi.RPC.LSP.Response;
using Rubberduck.RPC.Platform;
using System.Threading.Tasks;
using WebSocketSharp;

namespace Rubberduck.Server.Controllers
{
    public class CallHierarchyController : JsonRpcClient
    {
        public CallHierarchyController(WebSocket socket) : base(socket)
        {
        }

        /// <summary>
        /// Resolves incoming calls for a given call hierarchy item.
        /// </summary>
        /// <param name="parameters">The call hierarchy item to resolve.</param>
        /// <returns>A <c>CallHierarchyIncomingCall[]</c> object.</returns>
        [JsonRpcMethod(JsonRpcMethods.CallHierarchyIncoming)]
        public async Task<CallHierarchyIncomingCall[]> IncomingCalls(CallHierarchyIncomingCallsParams parameters)
        {
            return await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.CallHierarchyIncoming, parameters);
                var response = Request<CallHierarchyIncomingCall[]>(request);

                return response;
            });
        }

        /// <summary>
        /// Resolves outgoing calls for a given call hierarchy item.
        /// </summary>
        /// <param name="parameters">The call hierarchy item to resolve.</param>
        /// <returns>A <c>CallHierarchyOutgoingCall[]</c> object.</returns>
        [JsonRpcMethod(JsonRpcMethods.CallHierarchyOutgoing)]
        public async Task<CallHierarchyOutgoingCall[]> OutgoingCalls(CallHierarchyOutgoingCallsParams parameters)
        {
            return await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.CallHierarchyOutgoing, parameters);
                var response = Request<CallHierarchyOutgoingCall[]>(request);

                return response;
            });
        }
    }
}
