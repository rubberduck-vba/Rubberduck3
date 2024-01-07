using System;
using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.Model.Workspace
{
    public record class ProjectFile
    {
        public const string FileName = ".rdproj";
        public const string SourceRoot = ".src";

        /// <summary>
        /// The workspace root location where the project file is.
        /// </summary>
        /// <remarks>This property is not serialized.</remarks>
        [JsonIgnore]
        public Uri Uri { get; set; }
        [JsonIgnore]
        public string? ProjectId { get; set; }

        /// <summary>
        /// The Rubberduck version that created the file.
        /// </summary>
        public string Rubberduck { get; set; }

        /// <summary>
        /// Information about the VBA project.
        /// </summary>
        public Project VBProject { get; set; }
    }
}
