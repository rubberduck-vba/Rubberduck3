using System.IO;
using System.Runtime.InteropServices;
using Rubberduck.Unmanaged.Model;
using Rubberduck.Unmanaged.Abstract.SafeComWrappers.VB;
using System;

namespace Rubberduck.VBEditor.Extensions
{
    public static class ReferenceInfoExtensions
    {
        /// <summary>
        /// Gets the standard projectId for a <em>library reference</em>.
        /// <strong>Do not</strong> use this overload for <em>referenced user projects</em>.
        /// </summary>
        public static Uri GetWorkspaceUri(this ReferenceInfo reference)
        {
            return new QualifiedModuleName(reference).WorkspaceUri;
        }
    }

    public static class VBProjectExtensions
    {
        /// <summary>
        /// Gets the standard projectId for a <em>locked</em> user projects.
        /// <strong>Do not</strong> use this overload for <em>unlocked</em> user projects.
        /// </summary>
        public static Uri WorkspaceUri(this IVBProject _, string projectName, string projectPath)
        {
            return new QualifiedModuleName(projectName, projectPath, projectName).WorkspaceUri;
        }

        public static bool TryGetFullPath(this IVBProject project, out string? fullPath)
        {
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