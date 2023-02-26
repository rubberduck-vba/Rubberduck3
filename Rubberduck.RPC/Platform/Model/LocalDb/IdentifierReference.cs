using Rubberduck.InternalApi.Model;

namespace Rubberduck.RPC.Platform.Model.LocalDb
{
    public class IdentifierReference : DbEntity
    {
        public string TypeHint { get; set; }

        public int IsAssignmentTarget { get; set; }
        public int IsExplicitCallStatement { get; set; }
        public int IsExplicitLetAssignment { get; set; }
        public int IsSetAssignment { get; set; }
        public int IsReDim { get; set; }
        public int IsArrayAccess { get; set; }
        public int IsProcedureCoercion { get; set; }
        public int IsIndexedDefaultMemberAccess { get; set; }
        public int IsNonIndexedDefaultMemberAccess { get; set; }
        public int IsInnerRecursiveDefaultMemberAccess { get; set; }
        public int? DefaultMemberRecursionDepth { get; set; }

        public int ReferenceDeclarationId { get; set; }
        public int? QualifyingReferenceId { get; set; }

        public int? MemberDeclarationId { get; set; }
        public int ModuleDeclarationId { get; set; }
        public int ProjectDeclarationId { get; set; }

        public int ContextStartOffset { get; set; }
        public int ContextEndOffset { get; set; }
        public int IdentifierStartOffset { get; set; }
        public int IdentifierEndOffset { get; set; }
    }

    public class IdentifierReferenceInfo : IdentifierReference
    {
        public DeclarationType ReferenceDeclarationType { get; set; }
        public string ReferenceIdentifierName { get; set; }
        public int ReferenceIsUserDefined { get; set; }
        public string ReferenceDocString { get; set; }


        public DeclarationType? MemberDeclarationType { get; set; }
        public string MemberName { get; set; }

        public DeclarationType ModuleDeclarationType { get; set; }
        public string ModuleName { get; set; }
        public string Folder { get; set; }

        public string ProjectName { get; set; }
        public string VBProjectId { get; set; }
    }
}
