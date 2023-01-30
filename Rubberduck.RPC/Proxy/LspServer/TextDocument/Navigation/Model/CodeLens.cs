using Rubberduck.InternalApi.RPC.LSP;
using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.LspServer.TextDocument.Model;
using Rubberduck.RPC.Proxy.LspServer.Workspace.Model;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Model
{
    /// <summary>
    /// Represents a command that should be shown along with source text.
    /// </summary>
    /// <remarks>
    /// If no command is associated to it, the object is <em>unresolved</em>. Resolution is done in a second stage for performance reasons.
    /// </remarks>
    public class CodeLens
    {
        /// <summary>
        /// The range this code lens is valid. Should only span a single line.
        /// </summary>
        [JsonPropertyName("range"), LspCompliant]
        public Range Range { get; set; }

        /// <summary>
        /// The associated command.
        /// </summary>
        /// <remarks>
        /// <c>null</c> if unresolved.
        /// </remarks>
        [JsonPropertyName("command"), LspCompliant]
        public Command Command { get; set; }

        /// <summary>
        /// A data rentry field that is preserved for a subsequent resolve request.
        /// </summary>
        [JsonPropertyName("data"), LspCompliant]
        public object Data { get; set; }
    }
}
