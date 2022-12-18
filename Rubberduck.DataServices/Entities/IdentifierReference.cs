namespace Rubberduck.DataServices.Entities
{
    internal class IdentifierReference : DbEntity
    {
        public int IsMarkedForDeletion { get; set; }
        public int IsMarkedForUpdate { get; set; }

        public int ReferencedDeclarationId { get; set; }
        public int ParentDeclarationId { get; set; }
        public int? QualifyingReferenceId { get; set; }
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
        public string TypeHint { get; set; }
        public string Annotations { get; set; }
        
        public int DocumentLineStart { get; set; }
        public int DocumentLineEnd { get; set; }
        
        public int ContextStartOffset { get; set; }
        public int ContextEndOffset { get; set; }

        public int IdentifierStartOffset { get; set; }
        public int IdentifierEndOffset { get; set; }
    }
}
