using IOException = System.IO.IOException;
using System.Runtime.InteropServices;
using Rubberduck.Unmanaged.Model;
using System;

namespace Rubberduck.Unmanaged.Abstract.SafeComWrappers.VB
{
    public static class ReferenceInfoExtensions
    {
        /// <summary>
        /// Gets the standard projectId for a <em>library reference</em>.
        /// <strong>Do not</strong> use this overload for <em>referenced user projects</em>.
        /// </summary>
        public static Uri GetWorkspaceUri(this ReferenceInfo reference)
        {
            return new Uri(reference.FullPath);
        }
    }

    public static class VBProjectExtensions
    {
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