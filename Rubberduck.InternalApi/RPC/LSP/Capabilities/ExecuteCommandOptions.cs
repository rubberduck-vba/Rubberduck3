using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "executeCommandOptions")]
    public class ExecuteCommandOptions : WorkDoneProgressOptions
    {
        /// <summary>
        /// The commands to be executed on the server.
        /// </summary>
        [ProtoMember(2, Name = "commands")]
        public string[] Commands { get; set; }
    }
}
