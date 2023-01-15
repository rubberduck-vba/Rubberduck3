using Rubberduck.InternalApi.Model;
using System;

namespace Rubberduck.InternalApi.RPC.DataServer
{
    public class Project
    {
        public int Id { get; set; }
        public string VBProjectId { get; set; }
        public Guid? Guid { get; set; }
        public int? MajorVersion { get; set; }
        public int? MinorVersion { get; set; }
        public string Path { get; set; }

        public int DeclarationId { get; set; }
        public DeclarationType DeclarationType { get; set; }
        public string IdentifierName { get; set; }
        public bool IsUserDefined { get; set; }
    }
}
