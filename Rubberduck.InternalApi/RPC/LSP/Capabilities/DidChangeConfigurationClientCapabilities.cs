using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "didChangeConfigurationClientCapabilities")]
    public class DidChangeConfigurationClientCapabilities
    {
        /// <summary>
        /// The modified configuration settings.
        /// </summary>
        [ProtoMember(1, Name = "settings")]
        public LSPAny Settings { get; set; }
    }
}
