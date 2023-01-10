using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP
{
    public interface IWorkDoneProgressParams
    {
        /// <summary>
        /// A token that the server can use to report work done progress.
        /// </summary>
        [ProtoMember(1, Name = "workDoneToken")]
        string WorkDoneToken { get; set; }
    }

    [ProtoContract]
    public class WorkDoneProgressParams : IWorkDoneProgressParams
    {
        /// <summary>
        /// A token that the server can use to report work done progress.
        /// </summary>
        [ProtoMember(1, Name = "workDoneToken")]
        public string WorkDoneToken { get; set; }
    }
}
