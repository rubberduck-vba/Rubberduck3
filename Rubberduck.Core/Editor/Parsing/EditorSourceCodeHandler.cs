using ICSharpCode.AvalonEdit.Document;
using Rubberduck.Parsing;
using Rubberduck.UI;
using Rubberduck.VBEditor;
using Rubberduck.VBEditor.SourceCodeHandling;
using System;
using System.IO;
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

        public string StringSource(QualifiedModuleName module) => GetDocument(module)?.Text ?? string.Empty;

        public TextReader SourceCode(QualifiedModuleName module) => GetDocument(module).CreateReader();

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
        }

        public void SubstituteCode(QualifiedModuleName module, string newCode)
        {
            var document = GetDocument(module) ?? throw new ArgumentOutOfRangeException(nameof(module));
            document.Text = newCode;
        }

        public void SetSelection(QualifiedModuleName module, Selection selection)
        {
            var document = _documentProvider.GetDocument(module);
            
            var startOffset = document.GetOffset(selection.StartLine, selection.StartColumn);
            var endOffset = document.GetOffset(selection.EndLine, selection.EndColumn);

            // TODO set document selection
        }

        public CodeString GetCurrentLogicalLine(QualifiedModuleName module)
        {
            throw new NotImplementedException();
        }

        public int GetContentHash(QualifiedModuleName module)
        {
            throw new NotImplementedException();
        }
    }
}