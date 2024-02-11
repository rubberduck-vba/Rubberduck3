using Rubberduck.Unmanaged.Abstract.SafeComWrappers.VB;
using System;

namespace Rubberduck.Unmanaged.TypeLibs.Abstract
{
    public interface ITypeLibWrapperProvider
    {
        ITypeLibWrapper TypeLibWrapperFromProject(Uri uri);
        ITypeLibWrapper TypeLibWrapperFromProject(IVBProject project);
    }
}
