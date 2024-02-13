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

    public ProjectFile(Uri? uri = null, Project? project = null)
    {
        Rubberduck = "3.0";

        Uri = uri!;
        VBProject = project ?? new();
    }

    /// <summary>
    /// The absolute workspace root location where the project file is.
    /// </summary>
    /// <remarks>This property is not serialized.</remarks>
    [JsonIgnore]
    public Uri Uri { get; init; }

    /// <summary>
    /// The Rubberduck version that created the file.
    /// </summary>
    public string Rubberduck { get; init; }

    /// <summary>
    /// Information about the VBA project.
    /// </summary>
    public Project VBProject { get; init; }

    public ProjectFile WithUri(Uri uri) => this with { Uri = uri };
}
