using ICSharpCode.AvalonEdit.Document;
using Rubberduck.InternalApi.Model;

namespace Rubberduck.Parsing
{
    public interface ITextDocumentProvider
    {
        TextDocument GetDocument(IQualifiedModuleName module);
    }
}
