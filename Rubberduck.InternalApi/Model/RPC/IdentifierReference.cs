namespace Rubberduck.InternalApi.Model.RPC
{
    public class IdentifierReference
    {
        public bool IsMarkedForDeletion { get; set; }
        public bool IsMarkedForUpdate { get; set; }

        public int Id { get; set; }

        public Declaration ReferencedDeclaration { get; set; }
        public Declaration ParentDeclaration { get; set; }
        public IdentifierReference QualifyingReference { get; set; }
        public LocationInfo LocationInfo { get; set; }
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
        public string TypeHint { get; set; }
        public string[] Annotations { get; set; }
    }
}
