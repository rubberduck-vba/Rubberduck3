using ProtoBuf;
using Rubberduck.InternalApi.RPC.LSP.Response;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    [ProtoContract(Name = "typeHierarchySupertypesParams")]
    public class TypeHierarchySupertypesParams : WorkDoneProgressParams, IPartialResultParams
    {
        /// <summary>
        /// A token that the server can use to report partial results (e.g. streaming) to the client.
        /// </summary>
        [ProtoMember(2, Name = "partialResultToken")]
        public string PartialResultToken { get; set; }

        [ProtoMember(3, Name = "item")]
        public TypeHierarchyItem Item { get; set; }
    }
}
