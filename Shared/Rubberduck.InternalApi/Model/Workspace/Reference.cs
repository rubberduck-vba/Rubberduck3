using System;

namespace Rubberduck.InternalApi.Model.Workspace;

public record class Reference
{
    public string Name { get; set; }
    /// <summary>
    /// The absolute path to the referenced project or library.
    /// </summary>
    public string? Uri { get; set; }
    public Guid? Guid { get; set; }
    public int? Major { get; set; }
    public int? Minor { get; set; }
    public string? TypeLibInfoUri { get; set; }

    public bool IsUnremovable { get; set; }
}
