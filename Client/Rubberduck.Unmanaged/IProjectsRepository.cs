using Rubberduck.Unmanaged.Model.Abstract;
using System;

namespace Rubberduck.Unmanaged
{
    public interface IProjectsRepository : IProjectsProvider
    {
        void Refresh();
        void Refresh(Uri uri);
        void RemoveComponent(IQualifiedModuleName qualifiedModuleName);
    }
}
