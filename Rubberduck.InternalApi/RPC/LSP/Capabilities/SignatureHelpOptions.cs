using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "signatureHelpOptions")]
    public class SignatureHelpOptions : WorkDoneProgressOptions
    {
        /// <summary>
        /// The characters that trigger signature help automatically.
        /// </summary>
        [ProtoMember(2, Name = "triggerCharacters")]
        public string[] TriggerCharacters { get; set; }

        /// <summary>
        /// List of characters that re-trigger signature help.
        /// </summary>
        /// <remarks>
        /// These trigger characters are only active when signature help is already showing. 
        /// All trigger characters are also re-trigger characters.
        /// </remarks>
        [ProtoMember(3, Name = "retriggerCharacters")]
        public string[] RetriggerCharacters { get; set; }
    }
}
