using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.SharedServices.WorkDoneProgress.Configuration;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Editor.Configuration
{
    /// <summary>
    /// Represents document formatting configuration settings.
    /// </summary>
    public class DocumentFormattingOptions : WorkDoneProgressOptions
    {
        /// <summary>
        /// The size of a tab, in spaces.
        /// </summary>
        [JsonPropertyName("tabSize"), LspCompliant]
        public uint TabSize { get; set; }

        /// <summary>
        /// Whether to prefer spaces over tabs.
        /// </summary>
        [JsonPropertyName("insertSpaces"), LspCompliant]
        public bool InsertSpaces { get; set; } = true;

        /// <summary>
        /// Whether to trim trailing whitespace on a line.
        /// </summary>
        [JsonPropertyName("trimTrailingWhitespace"), LspCompliant]
        public bool TrimTrailingWhitespace { get; set; } = true;

        /// <summary>
        /// Whether to insert a newline character at the end of the file if one does not exist.
        /// </summary>
        [JsonPropertyName("insertFinalNewline"), LspCompliant]
        public bool InsertFinalNewline { get; set; } = true;

        /// <summary>
        /// Whether to trim all newlines after the final newline at the end of the file.
        /// </summary>
        [JsonPropertyName("trimFinalNewlines"), LspCompliant]
        public bool TrimFinalNewlines { get; set; } = true;

        /* additional properties don't have a named property in LSP specs. this complicates serialization a bit:
         * 
         *  [key: string]: boolean | integer | string
         *  
         *  since we cannot use objects here, to be LSP-compliant the indenter settings will have to be deconstructed.
         *  ...or the formatting options could implement IIndenterSettings.
         *  ...or there's a public class IndenterSettingsAdapter : IIndenterSettings to serialize on this side.
         */
    }
}
