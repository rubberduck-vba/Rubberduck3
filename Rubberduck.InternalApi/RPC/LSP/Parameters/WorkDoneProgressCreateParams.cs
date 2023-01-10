using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    [ProtoContract(Name = "workDoneProgressCreateParams")]
    public class WorkDoneProgressCreateParams
    {
        /// <summary>
        /// The token to be used to report progress for this task.
        /// </summary>
        [ProtoMember(1, Name = "token")]
        public string ProgressToken { get; set; }
    }
}
