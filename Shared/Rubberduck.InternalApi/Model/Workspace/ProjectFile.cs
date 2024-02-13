using System;
using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.Model.Workspace;

public record class ProjectFile
{
    /// <summary>
    /// The file name of a <em>Rubberduck project file</em>.
    /// </summary>
    public const string FileName = ".rdproj";
    /// <summary>
    /// The name of the <em>source root</em> folder in workspaces.
    /// </summary>
    public const string SourceRoot = ".src";

    /// <summary>
    /// The absolute workspace root location where the project file is.
    /// </summary>
    /// <remarks>This property is not serialized.</remarks>
    [JsonIgnore]
    public Uri Uri { get; init; } = default!;

    /// <summary>
    /// The Rubberduck version that created the file.
    /// </summary>
    public string Rubberduck { get; init; } = "3.0";

    /// <summary>
    /// Information about the VBA project.
    /// </summary>
    public Project VBProject { get; init; } = new();

    public ProjectFile WithUri(Uri uri) => this with { Uri = uri };
}
