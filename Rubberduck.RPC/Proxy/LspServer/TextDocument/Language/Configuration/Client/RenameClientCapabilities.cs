using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Configuration.Client
{
    public class RenameClientCapabilities
    {
        /// <summary>
        /// Whether the client supports dynamic registration.
        /// </summary>
        [JsonPropertyName("dynamicRegistration"), LspCompliant]
        public bool SupportsDynamicRegistration { get; set; }

        /// <summary>
        /// Whether client supports testing for validity of rename operations before execution.
        /// </summary>
        [JsonPropertyName("prepareSupport"), LspCompliant]
        public bool SupportsPreparation { get; set; }

        /// <summary>
        /// See <c>Constants.PrepareSupportDefaultBehavior</c>.
        /// </summary>
        [JsonPropertyName("prepareSupportDefaultBehavior"), JsonConverter(typeof(JsonStringEnumConverter)), LspCompliant]
        public Constants.PrepareSupportDefaultBehavior.AsEnum PrepareSupportDefaultBehavior { get; set; } 
            = Constants.PrepareSupportDefaultBehavior.AsEnum.Identifier;

        /// <summary>
        /// Whether the client honors the change annotations in text edits and resource operations returned via the rename request's workspace edit.
        /// </summary>
        [JsonPropertyName("honorsChangeAnnotations"), LspCompliant]
        public bool HonorsChangeAnnotations { get; set; }
    }
}
