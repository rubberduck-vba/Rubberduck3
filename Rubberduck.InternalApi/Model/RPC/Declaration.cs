using System.Collections.Generic;

namespace Rubberduck.InternalApi.Model.RPC
{
    public class Declaration
    {
        public bool IsMarkedForDeletion { get; set; }
        public bool IsMarkedForUpdate { get; set; }

        public int Id { get; set; }
        public LocationInfo LocationInfo { get; set; }
        public InternalApi.Model.DeclarationType DeclarationType { get; set; }
        public bool IsArray { get; set; }
        public string AsTypeName { get; set; }
        public Declaration AsTypeDeclaration { get; set; }
        public string TypeHint { get; set; }
        public string IdentifierName { get; set; }
        public bool IsUserDefined { get; set; }
        public Declaration ParentDeclaration { get; set; }
        public string DocString { get; set; }
        public string[] Annotations { get; set; }
        public string[] Attributes { get; set; }

        public IEnumerable<IdentifierReference> IdentifierReferences { get; set; } = new IdentifierReference[] { };
    }
}
