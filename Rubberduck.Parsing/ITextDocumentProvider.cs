using ICSharpCode.AvalonEdit.Document;
using Rubberduck.VBEditor;

namespace Rubberduck.Parsing
{
    public interface ITextDocumentProvider
    {
        TextDocument GetDocument(QualifiedModuleName module);
    }
}
