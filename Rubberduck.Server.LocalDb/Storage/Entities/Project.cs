using Rubberduck.InternalApi.Model;
using Rubberduck.Server.LocalDb.Internal;

namespace Rubberduck.DataServer.Storage.Entities
{
    internal class Project : DbEntity
    {
        public int DeclarationId { get; set; }
        public string VBProjectId { get; set; }
        public string Guid { get; set; }
        public int? MajorVersion { get; set; }
        public int? MinorVersion { get; set; }
        public string Path { get; set; }
    }

    internal class ProjectInfo : Project
    {
        public DeclarationType DeclarationType { get; set; }
        public string IdentifierName { get; set; }
        public string DocString { get; set; }
        public bool IsUserDefined { get; set; }
    }
}
