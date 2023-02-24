using Rubberduck.InternalApi.Model;

namespace Rubberduck.VBEditor.SourceCodeHandling
{
    public interface ISourceCodeProvider<TContent>
    {
        TContent SourceCode(IQualifiedModuleName module);
        string StringSource(IQualifiedModuleName module);
        int GetContentHash(IQualifiedModuleName module);
    }
}
