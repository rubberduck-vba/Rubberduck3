using IOException = System.IO.IOException;
using System.Runtime.InteropServices;
using Rubberduck.Unmanaged.Model;

namespace Rubberduck.Unmanaged.Abstract.SafeComWrappers
{
    public static class ReferenceInfoExtensions
    {
        /// <summary>
        /// Gets the standard projectId for a <em>library reference</em>.
        /// <strong>Do not</strong> use this overload for <em>referenced user projects</em>.
        /// </summary>
        public static string GetProjectId(this ReferenceInfo reference)
        {
            return new QualifiedModuleName(reference).ProjectId;
        }
    }

    public static class VBProjectExtensions
    {
        /// <summary>
        /// Gets the standard projectId for a <em>locked</em> user projects.
        /// <strong>Do not</strong> use this overload for <em>unlocked</em> user projects.
        /// </summary>
        public static string GetProjectId(this IVBProject project, string projectName, string projectPath)
        {
            return new QualifiedModuleName(projectName, projectPath, projectName).ProjectId;
        }

        /// <summary>
        /// Gets the standard projectId for an <em>unlocked</em> user projects.
        /// <strong>Do not</strong> use this overload for <em>locked</em> user projects.
        /// </summary>
        public static string GetProjectId(this IVBProject project)
        {
            if (project.IsWrappingNullReference)
            {
                return string.Empty;
            }

            var projectId = project.ProjectId;

            if (string.IsNullOrEmpty(projectId))
            {
                project.AssignProjectId();
                projectId = project.ProjectId;
            }

            return projectId;
        }

        public static bool TryGetFullPath(this IVBProject project, out string fullPath)
        {
            if (project.IsWrappingNullReference)
            {
                fullPath = null;
                return false;
            }

            try
            {
                fullPath = project.FileName;
            }
            catch (IOException)
            {
                // Filename throws exception if unsaved.
                fullPath = null;
                return false;
            }
            catch (COMException)
            {
                //LogManager.GetLogger(typeof(IVBProject).FullName).Warn(e);
                fullPath = null;
                return false;
            }

            return true;
        }
    }
}