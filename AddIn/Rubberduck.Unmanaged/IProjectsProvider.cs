using System;
using System.Collections.Generic;
using Rubberduck.Unmanaged.Abstract.SafeComWrappers.VB;
using Rubberduck.Unmanaged.Model.Abstract;

namespace Rubberduck.Unmanaged
{
    public interface IProjectsProvider : IDisposable
    {
        IVBProjects ProjectsCollection();
        IEnumerable<(Uri Uri, IVBProject Project)> Projects();
        IEnumerable<(Uri Uri, IVBProject Project)> LockedProjects();
        IVBProject Project(Uri Uri);
        IVBComponents ComponentsCollection(Uri workspaceUri);
        IEnumerable<(IQualifiedModuleName QualifiedModuleName, IVBComponent Component)> Components();
        IEnumerable<(IQualifiedModuleName QualifiedModuleName, IVBComponent Component)> Components(Uri workspaceUri);
        IVBComponent Component(IQualifiedModuleName qualifiedModuleName);
    }
}
