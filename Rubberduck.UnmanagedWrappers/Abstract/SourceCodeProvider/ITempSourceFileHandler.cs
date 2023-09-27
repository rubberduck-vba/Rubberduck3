using Rubberduck.Unmanaged.Abstract.SafeComWrappers;

namespace Rubberduck.Unmanaged.Abstract.SourceCodeProvider
{
    public interface ITempSourceFileHandler
    {
        string Export(IVBComponent component);
        IVBComponent ImportAndCleanUp(IVBComponent component, string fileName);

        string Read(IVBComponent component);
    }
}
