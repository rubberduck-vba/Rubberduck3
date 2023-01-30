using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Server.Commands.Parameters
{
    public interface IWorkDoneProgressParams
    {
        /// <summary>
        /// A token that the server can use to report work done progress.
        /// </summary>
        [JsonPropertyName("workDoneToken"), LspCompliant]
        string WorkDoneToken { get; set; }
    }


    public class WorkDoneProgressParams : IWorkDoneProgressParams
    {
        /// <summary>
        /// A token that the server can use to report work done progress.
        /// </summary>
        [JsonPropertyName("workDoneToken"), LspCompliant]
        public string WorkDoneToken { get; set; }
    }
}
