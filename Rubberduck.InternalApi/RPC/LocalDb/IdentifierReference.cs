using Rubberduck.InternalApi.Model;

namespace Rubberduck.InternalApi.RPC.DataServer
{

    public class IdentifierReference
    {
        public int Id { get; set; }
        public string TypeHint { get; set; }

        public int ReferenceDeclarationId { get; set; }
        public DeclarationType ReferenceDeclarationtype { get; set; }
        public bool ReferenceIsUserDefined { get; set; }
        public string ReferenceDocString { get; set; }

        public bool IsAssignmentTarget { get; set; }
        public bool IsExplicitCallStatement { get; set; }
        public bool IsExplicitLetAssignment { get; set; }
        public bool IsSetAssignment { get; set; }
        public bool IsReDim { get; set; }
        public bool IsArrayAccess { get; set; }
        public bool IsProcedureCoercion { get; set; }
        public bool IsIndexedDefaultMemberAccess { get; set; }
        public bool IsNonIndexedDefaultMemberAccess { get; set; }
        public bool IsInnerRecursiveDefaultMemberAccess { get; set; }
        public int? DefaultMemberRecursionDepth { get; set; }

        public int? QualifyingReferenceId { get; set; }
        public LocationInfo LocationInfo { get; set; }

        public int? MemberDeclarationId { get; set; }
        public DeclarationType MemberDeclarationType { get; set; }
        public string MemberName { get; set; }

        public int ModuleDeclarationId { get; set; }
        public DeclarationType ModuleDeclarationType { get; set; }
        public string ModuleName { get; set; }
        public string Folder { get; set; }

        public int ProjectDeclarationId { get; set; }
        public string ProjectName { get; set; }
        public string VBProjectId { get; set; }
    }
}
