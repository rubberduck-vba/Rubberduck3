using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    [ProtoContract(Name = "executeCommandParams")]
    public class ExecuteCommandParams
    {
        /// <summary>
        /// The identifier of the command to execute.
        /// </summary>
        [ProtoMember(1, Name = "command")]
        public string Command { get; set; }

        /// <summary>
        /// The arguments the command should be invoked with.
        /// </summary>
        [ProtoMember(2, Name = "arguments")]
        public LSPAny[] Arguments { get; set; }
    }
}
