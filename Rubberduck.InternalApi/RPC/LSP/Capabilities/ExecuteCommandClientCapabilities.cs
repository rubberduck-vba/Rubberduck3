using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "executeCommandClientCapabilities")]
    public class ExecuteCommandClientCapabilities
    {
        /// <summary>
        /// Whether the client supports dynamic registration for commands.
        /// </summary>
        [ProtoMember(1, Name = "dynamicRegistration")]
        public bool SupportsDynamicRegistration { get; set; }
    }
}
