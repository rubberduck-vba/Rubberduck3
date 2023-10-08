using Rubberduck.Unmanaged.Abstract.SafeComWrappers;

namespace Rubberduck.Unmanaged.Abstract.SourceCodeProvider
{
    public interface IComponentSourceCodeHandler
    {
        string SourceCode(IVBComponent module);
        IVBComponent SubstituteCode(IVBComponent module, string newCode);
    }
}