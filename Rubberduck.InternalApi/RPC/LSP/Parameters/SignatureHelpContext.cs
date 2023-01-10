using ProtoBuf;
using Rubberduck.InternalApi.RPC.LSP.Response;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    [ProtoContract(Name = "signatureHelpContext")]
    public class SignatureHelpContext
    {
        [ProtoMember(1, Name = "triggerKind")]
        public int TriggerKind { get; set; } = Constants.SignatureHelpTriggerKind.Invoked;

        /// <summary>
        /// The character that triggered signature help, if TriggerKind is 2.
        /// </summary>
        [ProtoMember(2, Name = "triggerCharacter")]
        public string TriggerCharacter { get; set; }

        /// <summary>
        /// <c>true</c> if signature help was already showing when it was triggered.
        /// </summary>
        [ProtoMember(3, Name = "isRetrigger")]
        public bool IsRetrigger { get; set; }

        [ProtoMember(4, Name = "activeSignatureHelp")]
        public SignatureHelp ActiveSignatureHelp { get; set; }
    }
}
