using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "windowWorkDoneProgressClientCapabilities")]
    public class WindowWorkDoneProgressClientCapabilities
    {
        /// <summary>
        /// Whether client supports server-initiated progress via a 'window/workDoneProgress/create' request.
        /// </summary>
        [ProtoMember(1, Name = "workDoneProgress")]
        public bool WorkDoneProgress { get; set; }
    }
}
