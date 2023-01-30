using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.LspServer.Workspace.Model;
using Rubberduck.RPC.Proxy.LspServer.Workspace.File.Model;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Model
{
    public class CodeAction
    {
        [JsonPropertyName("title"), LspCompliant]
        public string Title { get; set; }

        [JsonPropertyName("kind"), JsonConverter(typeof(JsonStringEnumConverter)), LspCompliant]
        public Constants.CodeActionKind.AsStringEnum Kind { get; set; }

        /// <summary>
        /// The diagnostics that this code action resolves.
        /// </summary>
        [JsonPropertyName("diagnostics"), LspCompliant]
        public Diagnostic[] Diagnostics { get; set; }

        /// <summary>
        /// Marks this as a preferred action; used by 'auto fix' command and can be targeted by keybindings.
        /// </summary>
        [JsonPropertyName("isPreferred"), LspCompliant]
        public bool IsPreferred { get; set; }

        /// <summary>
        /// Marks that the code action cannot currently be applied.
        /// </summary>
        [JsonPropertyName("disabled"), LspCompliant]
        public DisabledAction Disabled { get; set; }

        public class DisabledAction
        {
            /// <summary>
            /// Human-readable description of why the code action is currently disabled.
            /// </summary>
            [JsonPropertyName("reason"), LspCompliant]
            public string Reason { get; set; }
        }

        /// <summary>
        /// The workspace edit this code action performs, if any.
        /// </summary>
        /// <remarks>
        /// If a command is also specified, the edit is applied first.
        /// </remarks>
        [JsonPropertyName("edit"), LspCompliant]
        public WorkspaceEdit Edit { get; set; }

        /// <summary>
        /// The command this code action executes, if any.
        /// </summary>
        /// <remarks>
        /// If an edit is also specified, the command executes after the edit is applied.
        /// </remarks>
        [JsonPropertyName("command"), LspCompliant]
        public Command Command { get; set; }

        /// <summary>
        /// A data entry field that is preserved between a /codeAction and a /resolve request.
        /// </summary>
        [JsonPropertyName("data"), LspCompliant]
        public object Data { get; set; }
    }
}