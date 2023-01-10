using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP
{
    [ProtoContract]
    public interface IPartialResultParams
    {
        /// <summary>
        /// A token that the server can use to report partial results (e.g. streaming) to the client.
        /// </summary>
        [ProtoMember(1, Name = "partialResultToken")]
        string PartialResultToken { get; set; }
    }

    [ProtoContract(Name = "partialResultParams")]
    public class PartialResultParams : IPartialResultParams
    {
        /// <summary>
        /// A token that the server can use to report partial results (e.g. streaming) to the client.
        /// </summary>
        [ProtoMember(1, Name = "partialResultToken")]
        public string PartialResultToken { get; set; }
    }
}
