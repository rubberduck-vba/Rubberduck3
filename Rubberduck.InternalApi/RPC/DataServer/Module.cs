using Rubberduck.InternalApi.Model;

namespace Rubberduck.InternalApi.RPC.DataServer
{
    public class Module
    {
        public int Id { get; set; }
        public string Folder { get; set; }

        public int DeclarationId { get; set; }
        public DeclarationType DeclarationType { get; set; }
        public string IdentifierName { get; set; }
        public string DocString { get; set; }
        public bool IsUserDefined { get; set; }

        public int ProjectDeclarationId { get; set; }
        public string ProjectName { get; set; }
        public string VBProjectId { get; set; }
    }
}
