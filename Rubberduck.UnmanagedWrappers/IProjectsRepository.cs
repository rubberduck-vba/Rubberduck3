using Rubberduck.Unmanaged.Abstract;
using Rubberduck.Unmanaged.Model.Abstract;

namespace Rubberduck.Unmanaged
{
    public interface IProjectsRepository : IProjectsProvider
    {
        void Refresh();
        void Refresh(string projectId);
        void RemoveComponent(IQualifiedModuleName qualifiedModuleName);
    }
}
