using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "renameOptions")]
    public class RenameOptions : WorkDoneProgressOptions
    {
        /// <summary>
        /// If <c>true</c>, renames should be prepared and confirmed by the user before being executed.
        /// </summary>
        [ProtoMember(2, Name = "prepareProvider")]
        public bool IsPrepareProvider { get; set; }
    }
}
