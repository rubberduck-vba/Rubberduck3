using AustinHarris.JsonRpc;
using Rubberduck.InternalApi.RPC.LSP;
using Rubberduck.InternalApi.RPC.LSP.Parameters;
using Rubberduck.InternalApi.RPC.LSP.Response;
using Rubberduck.RPC.Platform;
using System.Threading.Tasks;
using WebSocketSharp;

namespace Rubberduck.Server.Controllers
{
    public class TypeHierarchyController : JsonRpcClient
    {
        public TypeHierarchyController(WebSocket socket) : base(socket)
        {
        }

        /// <summary>
        /// Resolves the supertypes (base classes) for a given type hierarchy item.
        /// </summary>
        /// <param name="parameters">The type hierarchy item to resolve</param>
        /// <returns>An array of <c>TypeHierarchyItem</c> objects.</returns>
        [JsonRpcMethod(JsonRpcMethods.SuperTypes)]
        public async Task<TypeHierarchyItem[]> SuperTypes(TypeHierarchySupertypesParams parameters)
        {
            return await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.SuperTypes, parameters);
                var response = Request<TypeHierarchyItem[]>(request);

                return response;
            });
        }

        /// <summary>
        /// Resolves the subtypes (derived classes) for a given type hierarchy item.
        /// </summary>
        /// <param name="parameters">The type hierarchy item to resolve</param>
        /// <returns>An array of <c>TypeHierarchyItem</c> objects.</returns>
        [JsonRpcMethod(JsonRpcMethods.SubTypes)]
        public async Task<TypeHierarchyItem[]> SubTypes(TypeHierarchySubtypesParams parameters)
        {
            return await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.SubTypes, parameters);
                var response = Request<TypeHierarchyItem[]>(request);

                return response;
            });
        }
    }
}
