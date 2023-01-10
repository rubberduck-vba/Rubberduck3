using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "documentOnTypeFormattingOptions")]
    public class DocumentOnTypeFormattingOptions
    {
        /// <summary>
        /// A character on which formatting should be triggered, like '{'.
        /// </summary>
        [ProtoMember(1, Name = "firstTriggerCharacter")]
        public string FirstTriggerCharacter { get; set; }

        [ProtoMember(2, Name = "moreTriggerCharacters")]
        public string[] MoreTriggerCharacters { get; set; }
    }
}
