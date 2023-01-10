using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "didChangeWatchedFilesClientCapabilities")]
    public class DidChangeWatchedFilesClientCapabilities
    {
        /// <summary>
        /// Whether the client supports dynamic registration for DidChangeWatchedFiles notifications.
        /// </summary>
        /// <remarks>
        /// Protocol does not support static configuration for server-side file changes.
        /// </remarks>
        [ProtoMember(1, Name = "dynamicRegistration")]
        public bool SupportsDynamicRegistration { get; set; }

        /// <summary>
        /// <c>true</c> if the client supports relative patterns.
        /// </summary>
        [ProtoMember(2, Name = "relativePatternSupport")]
        public bool SupportsRelativePattern { get; set; }
    }
}
