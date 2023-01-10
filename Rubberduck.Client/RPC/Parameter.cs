using ProtoBuf;
using Rubberduck.InternalApi.Model;

namespace Rubberduck.Client.RPC
{
    [ProtoContract(Name = "parameter")]
    public class Parameter
    {
        [ProtoMember(1)]
        public int Id { get; set; }
        [ProtoMember(2)]
        public int DeclarationId { get; set; }
        [ProtoMember(3)]
        public int? ParentDeclarationId { get; set; }

        [ProtoMember(4)]
        public int OrdinalPosition { get; set; }
        [ProtoMember(5)]
        public bool IsParamArray { get; set; }
        [ProtoMember(6)]
        public bool IsOptional { get; set; }
        [ProtoMember(7)]
        public bool IsByRef { get; set; }
        [ProtoMember(8)]
        public bool IsByVal { get; set; }
        [ProtoMember(9)]
        public bool IsModifierImplicit { get; set; }
        [ProtoMember(10)]
        public string DefaultValue { get; set; }

        [ProtoMember(11)]
        public DeclarationType DeclarationType { get; set; }
        [ProtoMember(12)]
        public string IdentifierName { get; set; }
        [ProtoMember(13)]
        public string DocString { get; set; }
        [ProtoMember(14)]
        public bool IsUserDefined { get; set; }

        [ProtoMember(15)]
        public DeclarationType MemberDeclarationType { get; set; }
        [ProtoMember(16)]
        public string MemberName { get; set; }

        [ProtoMember(17)]
        public DeclarationType ModuleDeclarationType { get; set; }
        [ProtoMember(18)]
        public string ModuleName { get; set; }
        [ProtoMember(19)]
        public string Folder { get; set; }

        [ProtoMember(20)]
        public string ProjectName { get; set; }
        [ProtoMember(21)]
        public string VBProjectId { get; set; }
    }
}
