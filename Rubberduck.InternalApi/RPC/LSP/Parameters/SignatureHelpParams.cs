using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    [ProtoContract(Name = "signatureHelpParams")]
    public class SignatureHelpParams : TextDocumentPositionParams, IWorkDoneProgressParams
    {
        /// <summary>
        /// A token that the server can use to report work done progress.
        /// </summary>
        [ProtoMember(3, Name = "workDoneToken")]
        public string WorkDoneToken { get; set; }

        [ProtoMember(4, Name = "context")]
        public SignatureHelpContext Context { get; set; }
    }
}
