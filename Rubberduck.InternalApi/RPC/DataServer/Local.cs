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
        public bool IsAutoAssigned { get; set; }
        [ProtoMember(3)]
        public string ValueExpression { get; set; }

        [ProtoMember(4)]
        public int DeclarationId { get; set; }
        [ProtoMember(5)]
        public DeclarationType DeclarationType { get; set; }
        [ProtoMember(6)]
        public string IdentifierName { get; set; }
        [ProtoMember(7)]
        public string DocString { get; set; }
        [ProtoMember(8)]
        public bool IsUserDefined { get; set; }
        [ProtoMember(9)]
        public string AsTypeName { get; set; }
        [ProtoMember(10)]
        public int AsTypeDeclarationId { get; set; }
        [ProtoMember(11)]
        public bool IsArray { get; set; }
        [ProtoMember(12)]
        public bool IsImplicit { get; set; }
        [ProtoMember(13)]
        public string TypeHint { get; set; }
        [ProtoMember(14)]
        public LocationInfo LocationInfo { get; set; }

        [ProtoMember(15)]
        public int ModuleDeclarationId { get; set; }
        [ProtoMember(16)]
        public DeclarationType ModuleDeclarationType { get; set; }
        [ProtoMember(17)]
        public string ModuleName { get; set; }
        [ProtoMember(18)]
        public string Folder { get; set; }

        [ProtoMember(19)]
        public int ProjectDeclarationId { get; set; }
        [ProtoMember(20)]
        public string ProjectName { get; set; }
        [ProtoMember(21)]
        public string VBProjectId { get; set; }
    }
}
