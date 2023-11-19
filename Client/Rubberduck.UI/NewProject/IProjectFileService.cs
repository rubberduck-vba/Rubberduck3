using Rubberduck.InternalApi.Model;
using System;

namespace Rubberduck.UI.NewProject
{
    public interface IProjectFileService
    {
        void CreateFile(ProjectFile model);
        ProjectFile ReadFile(Uri root);
    }

    public interface IWorkspaceFolderService
    {
        void CreateWorkspace(string path);
    }

    public class ProjectFileService : IProjectFileService
    {
        public void CreateFile(ProjectFile model)
        {
            throw new NotImplementedException();
        }

        public ProjectFile ReadFile(Uri root)
        {
            throw new NotImplementedException();
        }
    }

    public class WorkspaceFolderService : IWorkspaceFolderService
    {
        public void CreateWorkspace(string path)
        {
            throw new NotImplementedException();
        }
    }
}
