using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "textDocumentSyncOptions")]
    public class TextDocumentSyncOptions
    {
        /// <summary>
        /// Whether open and close notifications are sent to the server.
        /// </summary>
        [ProtoMember(1, Name = "openClose")]
        public bool NotifyOpenClose { get; set; } = true;

        /// <summary>
        /// How change notifications are sent to the server.
        /// </summary>
        /// <remarks>
        /// See <c>Constants.TextDocumentSyncKind</c>.
        /// </remarks>
        [ProtoMember(2, Name = "change")]
        public int ChangeNotificationSyncKind { get; set; }
    }
}
