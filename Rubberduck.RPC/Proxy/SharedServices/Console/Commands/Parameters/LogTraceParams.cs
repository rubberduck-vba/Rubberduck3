using Rubberduck.RPC.Platform.Metadata;

namespace Rubberduck.RPC.Proxy.SharedServices.Console.Commands.Parameters
{
    public class LogTraceParams
    {
        /// <summary>
        /// The message to be logged.
        /// </summary>
        [LspCompliant("message")]
        public string Message { get; set; }
    }
}
