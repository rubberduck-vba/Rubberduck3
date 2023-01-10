using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "codeActionOptions")]
    public class CodeActionOptions : WorkDoneProgressOptions
    {
        /// <summary>
        /// CodeActionKinds that this server may return.
        /// </summary>
        [ProtoMember(2, Name = "codeActionKinds")]
        public string[] CodeActionKinds { get; set; }

        /// <summary>
        /// <c>true</c> if the server provides support to resolve additional information for a code action.
        /// </summary>
        [ProtoMember(3, Name = "resolveProvider")]
        public bool IsResolveProvider { get; set; }
    }
}
