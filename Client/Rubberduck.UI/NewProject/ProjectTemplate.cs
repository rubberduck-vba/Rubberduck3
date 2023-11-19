using Rubberduck.InternalApi.Model;
using System;

namespace Rubberduck.UI.NewProject
{
    public class ProjectTemplate
    {
        public static ProjectTemplate Default { get; } = new();

        public string Rubberduck { get; init; } = "3.0";
        public string Name { get; init; } = "Empty Project";

        public ProjectFile.Reference[] References { get; init; } = new[] { ProjectFile.Reference.VisualBasicForApplications };
        public ProjectFile.Module[] Modules { get; init; } = Array.Empty<ProjectFile.Module>();
        public Uri[] OtherFiles { get; init; } = Array.Empty<Uri>();
    }
}
