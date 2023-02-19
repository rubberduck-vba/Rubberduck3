using Rubberduck.InternalApi.Model;
using System;

namespace Rubberduck.Client.RPC
{
    public class Project
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public Guid? Guid { get; set; }
        public string VBProjectId { get; set; }
        public int? MajorVersion { get; set; }
        public int? MinorVersion { get; set; }

        public int DeclarationId { get; set; }
        public int? ParentDeclarationId { get; set; }
        public DeclarationType DeclarationType { get; set; }
        public string IdentifierName { get; set; }
        public string DocString { get; set; }
        public bool IsUserDefined { get; set; }
    }
}
