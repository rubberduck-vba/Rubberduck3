using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    [ProtoContract(Name = "messageActionItem")]
    public class MessageActionItem
    {
        /// <summary>
        /// A short title like 'Retry', 'Open Log', etc.
        /// </summary>
        [ProtoMember(1)]
        public string Title { get; set; }
    }
}
