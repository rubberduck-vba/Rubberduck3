using Rubberduck.InternalApi.Model.Workspace;
using System;

namespace Rubberduck.InternalApi.Services;


public interface IProjectFileService
{
    void CreateFile(ProjectFile model);
    ProjectFile ReadFile(Uri root);
}
