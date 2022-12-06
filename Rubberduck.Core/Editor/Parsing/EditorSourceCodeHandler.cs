using ICSharpCode.AvalonEdit.Document;
using Rubberduck.Parsing;
using Rubberduck.UI;
using Rubberduck.UI.Abstract;
using Rubberduck.VBEditor;
using Rubberduck.VBEditor.SourceCodeHandling;
using System;
using System.Linq;

namespace Rubberduck.Core.Editor
{
    public class EditorSourceCodeHandler : IEditorSourceCodeHandler
    {
        private readonly ITextDocumentProvider _documentProvider;

        public EditorSourceCodeHandler(ITextDocumentProvider documentProvider)
        {
            _documentProvider = documentProvider;
        }

        private TextDocument GetDocument(QualifiedModuleName module) => _documentProvider.GetDocument(module);

        public string SourceCode(QualifiedModuleName module) => GetDocument(module)?.Text ?? string.Empty;

        public void SubstituteCode(QualifiedModuleName module, CodeString newCode)
        {
            var document = GetDocument(module) ?? throw new ArgumentOutOfRangeException(nameof(module));

            var (snipStart, snipLen) = newCode.SnippetPosition.ToTextEditorSelection(document);

            var snipStartLine = document.GetLineByOffset(snipLen);
            var snipEndLine = document.GetLineByOffset(snipStart + snipLen);

            foreach (var number in Enumerable.Range(snipStartLine.LineNumber, snipEndLine.LineNumber - snipStartLine.LineNumber + 1))
            {
                var line = document.GetLineByNumber(number);
                document.Lines.Remove(line);
            }

            document.Insert(snipStart, newCode.Code);
            // TODO set caret position!
        }

        public void SubstituteCode(QualifiedModuleName module, string newCode)
        {
            var document = GetDocument(module) ?? throw new ArgumentOutOfRangeException(nameof(module));
            document.Text = newCode;
        }
    }
}