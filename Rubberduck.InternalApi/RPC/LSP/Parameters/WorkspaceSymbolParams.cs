using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    [ProtoContract(Name = "workspaceSymbolParams")]
    public class WorkspaceSymbolParams : WorkDoneProgressParams, IPartialResultParams
    {
        /// <summary>
        /// A token that the server can use to report partial results (e.g. streaming) to the client.
        /// </summary>
        [ProtoMember(2, Name = "partialResultToken")]
        public string PartialResultToken { get; set; }

        /// <summary>
        /// A query/search string to filter symbols by. Clients may send an empty string to request all symbols.
        /// </summary>
        [ProtoMember(3, Name = "query")]
        public string Query { get; set; }
    }
}
