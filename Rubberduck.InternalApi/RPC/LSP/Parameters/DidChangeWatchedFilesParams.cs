using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    [ProtoContract(Name = "didChangeWatchedFilesParams")]
    public class DidChangeWatchedFilesParams
    {
        [ProtoMember(1, Name = "changes")]
        public FileEvent[] Changes { get; set; }
    }
}
