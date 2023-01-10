using Rubberduck.InternalApi.RPC.LSP.Parameters;
using Rubberduck.InternalApi.RPC.LSP.Response;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Rubberduck.Server.Controllers
{
    [ServiceContract]
    public class CallHierarchyController
    {
        /// <summary>
        /// Resolves incoming calls for a given call hierarchy item.
        /// </summary>
        /// <param name="parameters">The call hierarchy item to resolve.</param>
        /// <returns>A <c>CallHierarchyIncomingCall</c> object.</returns>
        [OperationContract(Name = "callHierarchy/incomingCalls")]
        public async Task<CallHierarchyIncomingCall[]> IncomingCalls(CallHierarchyIncomingCallsParams parameters)
        {
            return null;
        }

        /// <summary>
        /// Resolves outgoing calls for a given call hierarchy item.
        /// </summary>
        /// <param name="parameters">The call hierarchy item to resolve.</param>
        /// <returns>A <c>CallHierarchyOutgoingCall</c> object.</returns>
        [OperationContract(Name = "callHierarchy/outgoingCalls")]
        public async Task<CallHierarchyOutgoingCall> OutgoingCalls(CallHierarchyOutgoingCallsParams parameters)
        {
            return null;
        }
    }
}
