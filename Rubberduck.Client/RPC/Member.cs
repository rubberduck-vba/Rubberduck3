using Rubberduck.InternalApi.Model;

namespace Rubberduck.Client.RPC
{
    public class Member
    {
        public int Id { get; set; }
        public int? ImplementsDeclarationId { get; set; }

        public Accessibility Accessibility { get; set; }
        public bool IsAutoAssigned { get; set; }
        public bool IsWithEvents { get; set; }
        public bool IsDimStmt { get; set; }
        public string ValueExpression { get; set; }

        public int DeclarationId { get; set; }
        public int? ParentDeclarationId { get; set; }
        public DeclarationType DeclarationType { get; set; }
        public string IdentifierName { get; set; }
        public string DocString { get; set; }
        public bool IsUserDefined { get; set; }

        public DeclarationType ModuleDeclarationType { get; set; }
        public string ModuleName { get; set; }
        public string Folder { get; set; }

        public string ProjectName { get; set; }
        public string VBProjectId { get; set; }
    }
}
