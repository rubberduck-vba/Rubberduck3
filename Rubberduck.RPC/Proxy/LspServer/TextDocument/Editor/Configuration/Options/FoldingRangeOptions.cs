using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.SharedServices.WorkDoneProgress.Configuration;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Editor.Configuration
{
    public class FoldingRangeOptions : WorkDoneProgressOptions
    {

        [JsonPropertyName("foldingRangeKinds"), JsonConverter(typeof(JsonStringEnumConverter)), RubberduckSP]
        public Constants.FoldingRangeKind.AsStringEnum[] FoldingRangeKinds { get; set; }
    }
}
