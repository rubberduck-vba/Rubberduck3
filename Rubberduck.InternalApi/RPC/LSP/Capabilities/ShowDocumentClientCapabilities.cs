using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "showDocumentClientCapabilities")]
    public class ShowDocumentClientCapabilities
    {
        /// <summary>
        /// <c>true</c> if the client supports showDocument requests.
        /// </summary>
        [ProtoMember(1, Name = "support")]
        public bool IsSupported { get; set; }
    }
}
