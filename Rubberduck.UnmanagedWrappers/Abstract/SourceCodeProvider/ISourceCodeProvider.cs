using Rubberduck.Unmanaged.Abstract;
using Rubberduck.Unmanaged.Model.Abstract;

namespace Rubberduck.Unmanaged.Abstract.SourceCodeProvider
{
    public interface ISourceCodeProvider<TContent>
    {
        TContent SourceCode(IQualifiedModuleName module);
        string StringSource(IQualifiedModuleName module);
        int GetContentHash(IQualifiedModuleName module);
    }
}
