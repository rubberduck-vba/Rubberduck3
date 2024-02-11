using Rubberduck.InternalApi.Model.Workspace;

namespace Rubberduck.InternalApi.Services;

public interface IWorkspaceFolderService
{
    void CreateWorkspaceFolders(ProjectFile projectFile);
    void CopyTemplateFiles(ProjectFile projectFile, string templateSourceRoot);
}
