using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "textDocumentClientCapabilities")]
    public class TextDocumentSyncClientCapabilities
    {
        /// <summary>
        /// Whether the client supports dynamic registration for text document synchronization.
        /// </summary>
        [ProtoMember(1, Name = "dynamicRegistration")]
        public bool SupportsDynamicRegistration { get; set; }

        /// <summary>
        /// Whether client supports sending willSave notifications.
        /// </summary>
        [ProtoMember(2, Name = "willSave")]
        public bool SupportsWillSave { get; set; }

        /// <summary>
        /// Whether client sending willSave request waits for a response providing textEdits to apply before saving the document.
        /// </summary>
        [ProtoMember(3, Name = "willSaveWaitUntil")]
        public bool SupportsWillSaveWaitUntil { get; set; }

        /// <summary>
        /// Whether the client supports sending didSave notifications.
        /// </summary>
        [ProtoMember(4, Name = "didSave")]
        public bool SupportsDidSave { get; set; }
    }
}
