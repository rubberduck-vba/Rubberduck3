using Rubberduck.InternalApi.Model;

namespace Rubberduck.Client.RPC
{
    public class Local
    {
        public int Id { get; set; }
        public int DeclarationId { get; set; }
        public int? ParentDeclarationId { get; set; }

        public bool IsImplicit { get; set; }
        public bool IsAutoAssigned { get; set; }
        public string ValueExpression { get; set; }

        public DeclarationType DeclarationType { get; set; }
        public string IdentifierName { get; set; }
        public string DocString { get; set; }
        public bool IsUserDefined { get; set; }

        public DeclarationType MemberDeclarationType { get; set; }
        public string MemberName { get; set; }

        public DeclarationType ModuleDeclarationType { get; set; }
        public string ModuleName { get; set; }
        public string Folder { get; set; }
        
        public string ProjectName { get; set; }
        public string VBProjectId { get; set; }
    }
}
