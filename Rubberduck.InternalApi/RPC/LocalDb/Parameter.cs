using Rubberduck.InternalApi.Model;

namespace Rubberduck.InternalApi.RPC.DataServer
{
    public class Parameter
    {
        public int Id { get; set; }

        public int OrdinalPosition { get; set; }
        public bool IsParamArray { get; set; }
        public bool IsOptional { get; set; }
        public bool IsByRef { get; set; }
        public bool IsByVal { get; set; }
        public bool IsModifierImplicit { get; set; }
        public string DefaultValue { get; set; }

        public int DeclarationId { get; set; }
        public DeclarationType DeclarationType { get; set; }
        public string IdentifierName { get; set; }
        public string DocString { get; set; }
        public bool IsUserDefined { get; set; }
        public string AsTypeName { get; set; }
        public int AsTypeDeclarationId { get; set; }
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
