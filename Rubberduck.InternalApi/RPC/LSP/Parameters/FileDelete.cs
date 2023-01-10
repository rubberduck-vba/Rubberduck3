using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    [ProtoContract(Name = "fileDelete")]
    public class FileDelete
    {
        [ProtoMember(1, Name = "uri")]
        public string Uri { get; set; }
    }
}
