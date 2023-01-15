using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    public class ApplyWorkspaceEditParams
    {
        /// <summary>
        /// An optional label for the workspace edit, presented in the user interface e.g. to label the operation in an undo stack.
        /// </summary>
        [JsonPropertyName("label")]
        public string Label { get; set; }

        /// <summary>
        /// The edits to apply.
        /// </summary>
        [JsonPropertyName("edit")]
        public WorkspaceEdit Edit { get; set; }
    }
}
