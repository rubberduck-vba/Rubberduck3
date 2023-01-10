using ProtoBuf;
using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    [ProtoContract(Name = "formattingOptions")]
    public class FormattingOptions
    {
        /// <summary>
        /// The size of a tab, in spaces.
        /// </summary>
        [JsonPropertyName("tabSize")]
        [ProtoMember(1)]
        public uint TabSize { get; set; }

        /// <summary>
        /// Whether to prefer spaces over tabs.
        /// </summary>
        [JsonPropertyName("insertSpaces")]
        [ProtoMember(2)]
        public bool InsertSpaces { get; set; } = true;

        /// <summary>
        /// Whether to trim trailing whitespace on a line.
        /// </summary>
        [JsonPropertyName("trimTrailingWhitespace")]
        [ProtoMember(3)]
        public bool TrimTrailingWhitespace { get; set; } = true;

        /// <summary>
        /// Whether to insert a newline character at the end of the file if one does not exist.
        /// </summary>
        [JsonPropertyName("insertFinalNewline")]
        [ProtoMember(4)]
        public bool InsertFinalNewline { get; set; } = true;

        /// <summary>
        /// Whether to trim all newlines after the final newline at the end of the file.
        /// </summary>
        [JsonPropertyName("trimFinalNewlines")]
        [ProtoMember(5)]
        public bool TrimFinalNewlines { get; set; } = true;

        /* additional properties don't have a proper property per LSP specs. this complicates serialization a bit:
         * 
         *  [key: string]: boolean | integer | string
         *  
         *  since we cannot use object here, indenter settings will have to be deconstructed.
         */
    }
}
