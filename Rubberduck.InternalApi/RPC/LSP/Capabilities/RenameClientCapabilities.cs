using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    public class RenameClientCapabilities
    {
        /// <summary>
        /// Whether the client supports dynamic registration.
        /// </summary>
        [JsonPropertyName("dynamicRegistration")]
        public bool SupportsDynamicRegistration { get; set; }

        /// <summary>
        /// Whether client supports testing for validity of rename operations before execution.
        /// </summary>
        [JsonPropertyName("prepareSupport")]
        public bool SupportsPreparation { get; set; }

        /// <summary>
        /// See <c>Constants.PrepareSupportDefaultBehavior</c>.
        /// </summary>
        [JsonPropertyName("prepareSupportDefaultBehavior")]
        public int PrepareSupportDefaultBehavior { get; set; } = Constants.PrepareSupportDefaultBehavior.Identifier;

        /// <summary>
        /// Whether the client honors the change annotations in text edits and resource operations returned via the rename request's workspace edit.
        /// </summary>
        [JsonPropertyName("honorsChangeAnnotations")]
        public bool HonorsChangeAnnotations { get; set; }
    }
}
