using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    [ProtoContract(Name = "showMessageRequestParams")]
    public class ShowMessageRequestParams : ShowMessageParams
    {
        [ProtoMember(3, Name = "actions")]
        public MessageActionItem[] Actions { get; set; }
    }
}
