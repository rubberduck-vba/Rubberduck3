using Rubberduck.Unmanaged.Model.Abstract;
using Rubberduck.Unmanaged.Abstract.SafeComWrappers.VB;

namespace Rubberduck.Unmanaged.Model
{
    public static class QualifiedModuleNameExtensions
    {
        public static bool TryGetProject(this IQualifiedModuleName moduleName, IVBE vbe, out IVBProject project)
        {
            using (var projects = vbe.VBProjects)
            {
                foreach (var item in projects)
                {
                    if (item.WorkspaceUri == moduleName.WorkspaceUri && item.Name == moduleName.ProjectName)
                    {
                        project = item;
                        return true;
                    }

                    item.Dispose();
                }

                project = null;
                return false;
            }
        }

        public static bool TryGetComponent(this IQualifiedModuleName moduleName, IVBE vbe, out IVBComponent component)
        {
            if (TryGetProject(moduleName, vbe, out var project))
            {
                using (project)
                using (var components = project.VBComponents)
                {
                    component = components[moduleName.ComponentName];
                    return true;
                }
            }

            component = null;
            return false;
        }
    }
}
