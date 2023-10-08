using Rubberduck.Unmanaged.Abstract.SafeComWrappers;

namespace Rubberduck.Unmanaged.TypeLibs.Abstract
{
    public interface ITypeLibWrapperProvider
    {
        ITypeLibWrapper TypeLibWrapperFromProject(string projectId);
        ITypeLibWrapper TypeLibWrapperFromProject(IVBProject project);
    }
}
