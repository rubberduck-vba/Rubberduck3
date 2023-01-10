using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    [ProtoContract(Name = "publishDiagnosticsParams")]
    public class PublishDiagnosticsParams
    {
        [ProtoMember(1, Name = "uri")]
        public string DocumentUri { get; set; }

        [ProtoMember(2, Name = "version")]
        public int? Version { get; set; }

        [ProtoMember(3, Name = "diagnostics")]
        public Diagnostic[] Diagnostics { get; set; }
    }
}
