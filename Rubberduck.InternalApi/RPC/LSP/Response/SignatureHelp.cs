using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Response
{
    public class SignatureHelp
    {
        /// <summary>
        /// One or more signatures.
        /// </summary>
        [JsonPropertyName("signatures")]
        public SignatureInformation[] Signatures { get; set; }

        /// <summary>
        /// The index (zero-based) of the active signature.
        /// </summary>
        [JsonPropertyName("activeSignature")]
        public uint? ActiveSignature { get; set; }

        /// <summary>
        /// The index (zero-based) of the active parameter.
        /// </summary>
        [JsonPropertyName("activeParameter")]
        public uint? ActiveParameter { get; set; }
    }

    public class CodeAction
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("kind")]
        public string Kind { get; set; } = Constants.CodeActionKind.Empty;

        /// <summary>
        /// The diagnostics that this code action resolves.
        /// </summary>
        [JsonPropertyName("diagnostics")]
        public Diagnostic[] Diagnostics { get; set; }

        /// <summary>
        /// Marks this as a preferred action; used by 'auto fix' command and can be targeted by keybindings.
        /// </summary>
        [JsonPropertyName("isPreferred")]
        public bool IsPreferred { get; set; }

        /// <summary>
        /// Marks that the code action cannoit currently be applied.
        /// </summary>
        [JsonPropertyName("disabled")]
        public DisabledAction Disabled { get; set; }

        public class DisabledAction
        {
            /// <summary>
            /// Human-readable description of why the code action is currently disabled.
            /// </summary>
            [JsonPropertyName("reason")]
            public string Reason { get; set; }
        }

        /// <summary>
        /// The workspace edit this code action performs, if any.
        /// </summary>
        /// <remarks>
        /// If a command is also specified, the edit is applied first.
        /// </remarks>
        [JsonPropertyName("edit")]
        public WorkspaceEdit Edit { get; set; }

        /// <summary>
        /// The command this code action executes, if any.
        /// </summary>
        /// <remarks>
        /// If an edit is also specified, the command executes after the edit is applied.
        /// </remarks>
        [JsonPropertyName("command")]
        public Command Command { get; set; }

        /// <summary>
        /// A data entry field that is preserved between a /codeAction and a /resolve request.
        /// </summary>
        [JsonPropertyName("data")]
        public LSPAny Data { get; set; }
    }

    public class ColorInformation
    {
        [JsonPropertyName("range")]
        public Range Range { get; set; }

        [JsonPropertyName("color")]
        public Color Color { get; set; }
    }

    public class Color
    {
        [JsonPropertyName("red")]
        public decimal Red { get; set; }

        [JsonPropertyName("green")]
        public decimal Green { get; set; }

        [JsonPropertyName("blue")]
        public decimal Blue { get; set; }

        [JsonPropertyName("alpha")]
        public decimal Alpha { get; set; }
    }

    public class ColorPresentation
    {
        /// <summary>
        /// The label of this color presentation, shown e.g. on a color picker header.
        /// </summary>
        [JsonPropertyName("label")]
        public string Label { get; set; }

        /// <summary>
        /// An edit applied to a document when selecting this presentation for the color.
        /// </summary>
        [JsonPropertyName("textEdit")]
        public TextEdit TextEdit { get; set; }

        /// <summary>
        /// An optional array of edits applied when selecting this color presentation.
        /// </summary>
        [JsonPropertyName("additionalTextEdits")]
        public TextEdit[] AdditionalTextEdits { get; set; }
    }

    public class LinkedEditingRanges
    {
        /// <summary>
        /// A list of ranges that can be renamed together. The ranges must have identical length and content, and cannot overlap.
        /// </summary>
        [JsonPropertyName("ranges")]
        public Range[] Ranges { get; set; }

        /// <summary>
        /// An optional regex pattern that describes valid contents for the given range. If no pattern is provided, client configuration's word pattern will be used.
        /// </summary>
        [JsonPropertyName("wordPattern")]
        public string WordPattern { get; set; }
    }

    public class WorkspaceSymbol
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("kind")]
        public int SymbolKind { get; set; }

        [JsonPropertyName("tags")]
        public int[] SymbolTags { get; set; }

        [JsonPropertyName("containerName")]
        public string Qualifier { get; set; }

        [JsonPropertyName("location")]
        public Location Location { get; set; }

        [JsonPropertyName("data")]
        public LSPAny Data { get; set; }
    }

    public class WorkspaceFolder
    {
        /// <summary>
        /// The associated URI for this workspace folder.
        /// </summary>
        [JsonPropertyName("documentUri")]
        public string DocumentUri { get; set; }

        /// <summary>
        /// The name of the workspace folder.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
