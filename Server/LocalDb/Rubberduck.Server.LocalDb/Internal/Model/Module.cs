using Rubberduck.InternalApi.Model;

namespace Rubberduck.Server.LocalDb.Internal.Model
{
    internal class Module : DbEntity
    {
        public int DeclarationId { get; set; }
        public string Folder { get; set; }
    }

    internal class ModuleInfo : Module
    {
        public DeclarationType DeclarationType { get; set; }
        public string IdentifierName { get; set; }
        public string DocString { get; set; }
        public bool IsUserDefined { get; set; }

        public int ProjectDeclarationId { get; set; }
        public string ProjectName { get; set; }
        public string VBProjectId { get; set; }
    }
}
