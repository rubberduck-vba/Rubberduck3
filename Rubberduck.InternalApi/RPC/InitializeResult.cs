using System;
using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC
{
    public class InitializeResult<TServerCapabilities> where TServerCapabilities : class, new()
    {
        [JsonPropertyName("capabilities")]
        public TServerCapabilities Capabilities { get; set; }

        [JsonPropertyName("serverInfo")]
        public ServerInformation ServerInfo { get; set; }

        public class ServerInformation
        {
            /// <summary>
            /// The process ID of the server.
            /// </summary>
            /// <remarks>
            /// <em>This member is not defined by LSP.</em>
            /// </remarks>
            [JsonPropertyName("processId")]
            public int ProcessId { get; set; }

            /// <summary>
            /// The start timestamp of the server, i.e. when the server started listening on its port.
            /// </summary>
            /// <remarks>
            /// <em>This member is not defined by LSP.</em>
            /// </remarks>
            [JsonPropertyName("startTimestamp")]
            public DateTime StartTimestamp { get; set; }

            /// <summary>
            /// The name of the server.
            /// </summary>
            [JsonPropertyName("name")]
            public string Name { get; set; }

            /// <summary>
            /// The version of the server.
            /// </summary>
            [JsonPropertyName("version")]
            public string Version { get; set; }
        }
    }
}
