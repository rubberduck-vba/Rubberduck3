using Rubberduck.InternalApi.Model;

namespace Rubberduck.Client.RPC
{
    public class Parameter
    {
        public int Id { get; set; }
        public int DeclarationId { get; set; }
        public int? ParentDeclarationId { get; set; }

        public int OrdinalPosition { get; set; }
        public bool IsParamArray { get; set; }
        public bool IsOptional { get; set; }
        public bool IsByRef { get; set; }
        public bool IsByVal { get; set; }
        public bool IsModifierImplicit { get; set; }
        public string DefaultValue { get; set; }

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
