using System;

namespace Rubberduck.InternalApi.Model.Workspace
{
    public class VBProjectInfo
    {
        public string Name { get; set; }
        public Uri WorkspaceUri { get; set; }
        public string? Location { get; set; }
        public bool IsLocked { get; set; }
        public bool HasWorkspace { get; set; }
        public bool HasSourceCode { get; set; }
    }
}
