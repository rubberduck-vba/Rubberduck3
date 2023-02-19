using Rubberduck.InternalApi.Model;

namespace Rubberduck.InternalApi.RPC.DataServer
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
        public DeclarationType DeclarationType { get; set; }
        public string IdentifierName { get; set; }
        public string DocString { get; set; }
        public bool IsUserDefined { get; set; }
        public string AsTypeName { get; set; }
        public int? AsTypeDeclarationId { get; set; }
        public bool IsArray { get; set; }
        public string TypeHint { get; set; }

        public LocationInfo LocationInfo { get; set; }
        public int ModuleDeclarationId { get; set; }
        public DeclarationType ModuleDeclarationType { get; set; }
        public string ModuleName { get; set; }
        public string Folder { get; set; }

        public int ProjectDeclarationId { get; set; }
        public string ProjectName { get; set; }
        public string VBProjectId { get; set; }
    }
}
