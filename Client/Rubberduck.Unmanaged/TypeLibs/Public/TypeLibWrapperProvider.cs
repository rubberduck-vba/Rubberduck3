using System.Runtime.InteropServices.ComTypes;
using Rubberduck.Unmanaged.Abstract.SafeComWrappers.VB;
using Rubberduck.Unmanaged.TypeLibs.Abstract;

namespace Rubberduck.Unmanaged.TypeLibs.Public
{
    public class TypeLibWrapperProvider : ITypeLibWrapperProvider
    {
        private readonly IProjectsProvider _projectsProvider;

        /// <summary>
        /// Simplifies the work of obtaining a <see cref="ITypeLib"/> for a given
        /// <see cref="IVBProject"/> and wraps in a <see cref="ITypeLibWrapper"/>
        /// in order to expose some VBE-specific extensions upon the type APIs.
        /// </summary>
        /// <param name="projectsProvider">
        /// Injected provider maintaining a collection of <see cref="IVBProject"/>s
        /// </param>
        public TypeLibWrapperProvider(IProjectsProvider projectsProvider)
        {
            _projectsProvider = projectsProvider;
        }

        public ITypeLibWrapper TypeLibWrapperFromProject(string projectId)
        {
            var project = _projectsProvider.Project(projectId);
            return TypeLibWrapperFromProject(project);
        }

        public ITypeLibWrapper TypeLibWrapperFromProject(IVBProject project)
        {
            return project != null ? TypeLibWrapper.FromVBProject(project) : null;
        }
    }
}