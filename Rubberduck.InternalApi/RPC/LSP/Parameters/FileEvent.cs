using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    [ProtoContract(Name = "fileEvent")]
    public class FileEvent
    {
        [ProtoMember(1, Name = "uri")]
        public string Uri { get; set; }

        /// <summary>
        /// See <c>Constants.FileChangeType</c>.
        /// </summary>
        [ProtoMember(2, Name = "type")]
        public int Type { get; set; }
    }
}
