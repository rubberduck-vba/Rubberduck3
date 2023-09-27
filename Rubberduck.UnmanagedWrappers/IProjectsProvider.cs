using System;
using System.Collections.Generic;
using Rubberduck.Unmanaged.Abstract;
using Rubberduck.Unmanaged.Abstract.SafeComWrappers;
using Rubberduck.Unmanaged.Model.Abstract;

namespace Rubberduck.Unmanaged
{
    public interface IProjectsProvider : IDisposable
    {
        IVBProjects ProjectsCollection();
        IEnumerable<(string ProjectId, IVBProject Project)> Projects();
        IEnumerable<(string ProjectId, IVBProject Project)> LockedProjects();
        IVBProject Project(string projectId);
        IVBComponents ComponentsCollection(string projectId);
        IEnumerable<(IQualifiedModuleName QualifiedModuleName, IVBComponent Component)> Components();
        IEnumerable<(IQualifiedModuleName QualifiedModuleName, IVBComponent Component)> Components(string projectId);
        IVBComponent Component(IQualifiedModuleName qualifiedModuleName);
    }
}
