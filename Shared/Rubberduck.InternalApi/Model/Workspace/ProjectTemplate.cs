namespace Rubberduck.InternalApi.Model.Workspace;

public record class ProjectTemplate
{
    public const string TemplateSourceFolderName = ".template";

    public static ProjectTemplate Default { get; } = new();

    public string Rubberduck { get; init; } = "3.0";
    public string Name { get; init; } = "EmptyProject";

    public ProjectFile ProjectFile { get; init; }
}
