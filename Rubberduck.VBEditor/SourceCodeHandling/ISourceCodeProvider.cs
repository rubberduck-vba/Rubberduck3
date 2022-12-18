namespace Rubberduck.VBEditor.SourceCodeHandling
{
    public interface ISourceCodeProvider<TContent>
    {
        TContent SourceCode(QualifiedModuleName module);
        string StringSource(QualifiedModuleName module);
    }
}
