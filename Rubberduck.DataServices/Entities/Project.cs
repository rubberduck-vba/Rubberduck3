using ProtoBuf;

namespace Rubberduck.DataServices.Entities
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
}
