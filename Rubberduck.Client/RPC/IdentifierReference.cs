using ProtoBuf;
using Rubberduck.InternalApi.Model;

namespace Rubberduck.Client.RPC
{
    [ProtoContract]
    public class IdentifierReference
    {
        [ProtoMember(1)]
        public int Id { get; set; }
        [ProtoMember(2)]
        public string TypeHint { get; set; }

        [ProtoMember(3)]
        public int ReferenceDeclarationId { get; set; }
        [ProtoMember(4)]
        public DeclarationType ReferenceDeclarationtype { get; set; }
        [ProtoMember(5)]
        public bool ReferenceIsUserDefined { get; set; }
        [ProtoMember(6)]
        public string ReferenceDocString { get; set; }

        [ProtoMember(7)]
        public bool IsAssignmentTarget { get; set; }
        [ProtoMember(8)]
        public bool IsExplicitCallStatement { get; set; }
        [ProtoMember(9)]
        public bool IsExplicitLetAssignment { get; set; }
        [ProtoMember(10)]
        public bool IsSetAssignment { get; set; }
        [ProtoMember(11)]
        public bool IsReDim { get; set; }
        [ProtoMember(12)]
        public bool IsArrayAccess { get; set; }
        [ProtoMember(13)]
        public bool IsProcedureCoercion { get; set; }
        [ProtoMember(14)]
        public bool IsIndexedDefaultMemberAccess { get; set; }
        [ProtoMember(15)]
        public bool IsNonIndexedDefaultMemberAccess { get; set; }
        [ProtoMember(16)]
        public bool IsInnerRecursiveDefaultMemberAccess { get; set; }
        [ProtoMember(17)]
        public int? DefaultMemberRecursionDepth { get; set; }

        [ProtoMember(18)]
        public int? QualifyingReferenceId { get; set; }
        [ProtoMember(19)]
        public LocationInfo LocationInfo { get; set; }

        [ProtoMember(20)]
        public int? MemberDeclarationId { get; set; }
        [ProtoMember(21)]
        public DeclarationType MemberDeclarationType { get; set; }
        [ProtoMember(22)]
        public string MemberName { get; set; }

        [ProtoMember(23)]
        public int ModuleDeclarationId { get; set; }
        [ProtoMember(24)]
        public DeclarationType ModuleDeclarationType { get; set; }
        [ProtoMember(25)]
        public string ModuleName { get; set; }
        [ProtoMember(26)]
        public string Folder { get; set; }

        [ProtoMember(27)]
        public int ProjectDeclarationId { get; set; }
        [ProtoMember(28)]
        public string ProjectName { get; set; }
        [ProtoMember(29)]
        public string VBProjectId { get; set; }
    }
}
