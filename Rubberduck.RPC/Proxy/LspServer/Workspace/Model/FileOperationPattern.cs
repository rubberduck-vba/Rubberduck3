using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.LspServer.Configuration.Options;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Workspace.Model
{
    public class FileOperationPattern
    {
        /**
         * Glob patterns can have the following syntax:
         * - `*` to match one or more characters in a path segment
         * - `?` to match on one character in a path segment
         * - `**` to match any number of path segments, including none
         * - `{}` to group sub patterns into an OR expression. (e.g. `**​/*.{ts,js}`
         *   matches all TypeScript and JavaScript files)
         * - `[]` to declare a range of characters to match in a path segment
         *   (e.g., `example.[0-9]` to match on `example.0`, `example.1`, …)
         * - `[!...]` to negate a range of characters to match in a path segment
         *   (e.g., `example.[!0-9]` to match on `example.a`, `example.b`, but
         *   not `example.0`)
         */

        /// <summary>
        /// The glob pattern to match.
        /// </summary>
        [JsonPropertyName("glob"), LspCompliant]
        public string Glob { get; set; }

        /// <summary>
        /// Whether to match files or folders with this pattern. Matches both if unspecified.
        /// </summary>
        /// <remarks>See <c>Constants.FileOperationPatternKind</c>.</remarks>
        [JsonPropertyName("matches"), LspCompliant]
        public string Matches { get; set; }

        /// <summary>
        /// Additional options used during matching.
        /// </summary>
        [JsonPropertyName("options"), LspCompliant]
        public FileOperationPatternOptions Options { get; set; }
    }
}
