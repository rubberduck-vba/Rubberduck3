using System;

namespace Rubberduck.InternalApi.Model.RPC
{
    public class Project
    {
        public int Id { get; set; }

        public Declaration Declaration { get; set; }
        public string VBProjectId { get; set; }
        public Guid? Guid { get; set; }
        public int? MajorVersion { get; set; }
        public int? MinorVersion { get; set; }
        public string Path { get; set; }
    }
}
