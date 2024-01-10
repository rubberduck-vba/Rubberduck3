using System;
using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.Model.Workspace
{
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
        public Uri Uri { get; set; }
        /// <summary>
        /// The ID string used by the VBIDE addin to uniquely identify this project.
        /// </summary>
        [JsonIgnore]
        public string? ProjectId { get; set; }

        /// <summary>
        /// The Rubberduck version that created the file.
        /// </summary>
        public string Rubberduck { get; set; } = "3.0";

        /// <summary>
        /// Information about the VBA project.
        /// </summary>
        public Project VBProject { get; set; } = new();
    }
}
