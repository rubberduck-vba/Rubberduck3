using ProtoBuf;
using Rubberduck.InternalApi.Model;

namespace Rubberduck.InternalApi.RPC.DataServer
{
    [ProtoContract(Name = "parameter")]
    public class Parameter
    {
        [ProtoMember(1)]
        public int Id { get; set; }

        [ProtoMember(2)]
        public int OrdinalPosition { get; set; }
        [ProtoMember(3)]
        public bool IsParamArray { get; set; }
        [ProtoMember(4)]
        public bool IsOptional { get; set; }
        [ProtoMember(5)]
        public bool IsByRef { get; set; }
        [ProtoMember(6)]
        public bool IsByVal { get; set; }
        [ProtoMember(7)]
        public bool IsModifierImplicit { get; set; }
        [ProtoMember(8)]
        public string DefaultValue { get; set; }

        [ProtoMember(9)]
        public int DeclarationId { get; set; }
        [ProtoMember(10)]
        public DeclarationType DeclarationType { get; set; }
        [ProtoMember(11)]
        public string IdentifierName { get; set; }
        [ProtoMember(12)]
        public string DocString { get; set; }
        [ProtoMember(13)]
        public bool IsUserDefined { get; set; }
        [ProtoMember(14)]
        public string AsTypeName { get; set; }
        [ProtoMember(15)]
        public int AsTypeDeclarationId { get; set; }
        [ProtoMember(16)]
        public bool IsArray { get; set; }
        [ProtoMember(17)]
        public string TypeHint { get; set; }
        [ProtoMember(18)]
        public LocationInfo LocationInfo { get; set; }

        [ProtoMember(19)]
        public int ModuleDeclarationId { get; set; }
        [ProtoMember(20)]
        public DeclarationType ModuleDeclarationType { get; set; }
        [ProtoMember(21)]
        public string ModuleName { get; set; }
        [ProtoMember(22)]
        public string Folder { get; set; }

        [ProtoMember(23)]
        public int ProjectDeclarationId { get; set; }
        [ProtoMember(24)]
        public string ProjectName { get; set; }
        [ProtoMember(25)]
        public string VBProjectId { get; set; }
    }
}
