using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "workspaceFolderServerCapabilities")]
    public class WorkspaceFoldersServerCapabilities
    {
        /// <summary>
        /// <c>true</c> if the server supports workspace folders.
        /// </summary>
        [ProtoMember(1, Name = "supported")]
        public bool IsSupported { get; set; }

        /// <summary>
        /// Whether the server wants to receive workspace folder change notifications.
        /// If a string other than 'true' or 'false' is provided, the string is treated as a client registration ID.
        /// </summary>
        [ProtoMember(2, Name = "changeNotifications")]
        public string ChangeNotifications { get; set; }
    }
}
