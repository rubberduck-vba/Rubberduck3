using Rubberduck.InternalApi.Model;

namespace Rubberduck.UI.NewProject
{
    public record class ProjectTemplate
    {
        public static ProjectTemplate Default { get; } = new();

        public string Rubberduck { get; init; } = "3.0";
        public string Name { get; init; } = "EmptyProject";

        public ProjectFile ProjectFile { get; init; }
    }
}
