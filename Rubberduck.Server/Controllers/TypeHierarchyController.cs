using Rubberduck.InternalApi.RPC.LSP.Parameters;
using Rubberduck.InternalApi.RPC.LSP.Response;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Rubberduck.Server.Controllers
{
    [ServiceContract]
    public class TypeHierarchyController
    {
        /// <summary>
        /// Resolves the supertypes (base classes) for a given type hierarchy item.
        /// </summary>
        /// <param name="parameters">The type hierarchy item to resolve</param>
        /// <returns>An array of <c>TypeHierarchyItem</c> objects.</returns>
        [OperationContract(Name = "typeHierarchy/supertypes")]
        public async Task<TypeHierarchyItem[]> SuperTypes(TypeHierarchySupertypesParams parameters)
        {
            return null;
        }

        /// <summary>
        /// Resolves the subtypes (derived classes) for a given type hierarchy item.
        /// </summary>
        /// <param name="parameters">The type hierarchy item to resolve</param>
        /// <returns>An array of <c>TypeHierarchyItem</c> objects.</returns>
        [OperationContract(Name = "typeHierarchy/subtypes")]
        public async Task<TypeHierarchyItem[]> SubTypes(TypeHierarchySubtypesParams parameters)
        {
            return null;
        }
    }
}
