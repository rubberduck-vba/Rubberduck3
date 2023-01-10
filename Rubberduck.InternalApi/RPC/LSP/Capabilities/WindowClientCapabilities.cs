using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "windowClientCapabilities")]
    public class WindowClientCapabilities
    {
        /// <summary>
        /// Whether the client supports server-initiated progress using the 'window/workDoneProgress/create' request.
        /// </summary>
        /// <remarks>
        /// Also controls whether client supports handling progress notifications.
        /// </remarks>
        [ProtoMember(1, Name = "workDoneProgress")]
        public bool WorkDoneProgress { get; set; }

        /// <summary>
        /// Capabilities specific to the showMessage request.
        /// </summary>
        [ProtoMember(2, Name = "showMessage")]
        public ShowMessageRequestClientCapabilities ShowMessage { get; set; }

        /// <summary>
        /// Capabilities specific to the showDocument request.
        /// </summary>
        [ProtoMember(3, Name = "showDocument")]
        public ShowDocumentClientCapabilities ShowDocument { get; set; }
    }
}
