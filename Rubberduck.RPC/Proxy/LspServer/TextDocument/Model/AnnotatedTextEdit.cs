using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Model
{
    /// <summary>
    /// A <c>TextEdit</c> with an additional change annotation.
    /// </summary>
    public class AnnotatedTextEdit : TextEdit
    {
        [JsonPropertyName("annotationId")]
        public string AnnotationId { get; set; }
    }
}
