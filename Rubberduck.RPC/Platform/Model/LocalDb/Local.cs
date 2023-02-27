using Rubberduck.InternalApi.Model;

namespace Rubberduck.RPC.Platform.Model.LocalDb
{
    public class Local : DbEntity
    {
        public int DeclarationId { get; set; }
        public int IsAutoAssigned { get; set; }
        public int IsImplicit { get; set; }
        public string ValueExpression { get; set; }
    }

    public class LocalInfo : Local
    {
        public DeclarationType DeclarationType { get; set; }
        public string IdentifierName { get; set; }
        public string DocString { get; set; }
        public bool IsUserDefined { get; set; }

        public string AsTypeName { get; set; }
        public int? AsTypeDeclarationId { get; set; }
        public bool IsArray { get; set; }
        public string TypeHint { get; set; }

        public int? ContextStartOffset { get; set; }
        public int? ContextEndOffset { get; set; }
        public int? IdentifierStartOffset { get; set; }
        public int? IdentifierEndOffset { get; set; }

        public int MemberDeclarationId { get; set; }
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