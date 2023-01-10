using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    [ProtoContract(Name = "workDoneProgressCancelParams")]
    public class WorkDoneProgressCancelParams
    {
        /// <summary>
        /// The token to be used to report progress for this task.
        /// </summary>
        [ProtoMember(2, Name = "token")]
        public string ProgressToken { get; set; }
    }
}
