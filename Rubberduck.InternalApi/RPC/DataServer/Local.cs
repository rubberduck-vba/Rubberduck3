using ProtoBuf;
using Rubberduck.InternalApi.Model;

namespace Rubberduck.InternalApi.RPC.DataServer
{
    [ProtoContract]
    public class Local
    {
        [ProtoMember(1)]
        public int Id { get; set; }
        [ProtoMember(2)]
        public int DeclarationId { get; set; }
        [ProtoMember(3)]
        public int? ParentDeclarationId { get; set; }

        [ProtoMember(4)]
        public bool IsImplicit { get; set; }
        [ProtoMember(5)]
        public bool IsAutoAssigned { get; set; }
        [ProtoMember(6)]
        public string ValueExpression { get; set; }

        [ProtoMember(7)]
        public DeclarationType DeclarationType { get; set; }
        [ProtoMember(8)]
        public string IdentifierName { get; set; }
        [ProtoMember(9)]
        public string DocString { get; set; }
        [ProtoMember(10)]
        public bool IsUserDefined { get; set; }

        [ProtoMember(11)]
        public DeclarationType MemberDeclarationType { get; set; }
        [ProtoMember(12)]
        public string MemberName { get; set; }

        [ProtoMember(13)]
        public DeclarationType ModuleDeclarationType { get; set; }
        [ProtoMember(14)]
        public string ModuleName { get; set; }
        [ProtoMember(15)]
        public string Folder { get; set; }

        [ProtoMember(16)]
        public string ProjectName { get; set; }
        [ProtoMember(17)]
        public string VBProjectId { get; set; }
    }
}
