using System;

namespace Rubberduck.InternalApi.Model.Workspace
{
    public record class Reference
    {
        public static Reference VisualBasicForApplications { get; } = new()
        {
            Name = "VBA",
            Uri = "C:\\Program Files\\Common Files\\Microsoft Shared\\VBA\\VBA7.1\\VBE7.DLL",
            IsUnremovable = true,
        };

        public string Name { get; set; }
        public string? Uri { get; set; }
        public Guid? Guid { get; set; }
        public int? Major { get; set; }
        public int? Minor { get; set; }
        public string? TypeLibInfoUri { get; set; }

        public bool IsUnremovable { get; set; }
    }
}
