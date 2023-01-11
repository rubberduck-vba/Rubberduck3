using ProtoBuf;
using Rubberduck.InternalApi.Model;

namespace Rubberduck.InternalApi.RPC.DataServer
{
    [ProtoContract]
    public class Module
    {
        [ProtoMember(1)]
        public int Id { get; set; }

        [ProtoMember(2)]
        public string Folder { get; set; }

        [ProtoMember(3)]
        public int DeclarationId { get; set; }

        [ProtoMember(4)]
        public DeclarationType DeclarationType { get; set; }

        [ProtoMember(5)]
        public string IdentifierName { get; set; }

        [ProtoMember(6)]
        public string DocString { get; set; }

        [ProtoMember(7)]
        public bool IsUserDefined { get; set; }

        [ProtoMember(8)]
        public int ProjectDeclarationId { get; set; }

        [ProtoMember(9)]
        public string ProjectName { get; set; }

        [ProtoMember(10)]
        public string VBProjectId { get; set; }
    }
}
