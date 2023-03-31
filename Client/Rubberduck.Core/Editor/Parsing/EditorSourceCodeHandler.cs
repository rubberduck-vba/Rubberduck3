using ICSharpCode.AvalonEdit.Document;
using Rubberduck.VBEditor;
using Rubberduck.VBEditor.SourceCodeHandling;
using Rubberduck.InternalApi.Model;
using System;
using System.IO;
using System.Linq;
using RubberduckUI.Extensions;
using Rubberduck.UI.Abstract;

namespace Rubberduck.Core.Editor
{
    public class EditorSourceCodeHandler : IEditorSourceCodeHandler
    {
        private readonly ITextDocumentProvider _documentProvider;

        public EditorSourceCodeHandler(ITextDocumentProvider documentProvider)
        {
            _documentProvider = documentProvider;
        }

        private TextDocument GetDocument(IQualifiedModuleName module) => null; //_documentProvider.GetDocument(module);

        public string StringSource(IQualifiedModuleName module) => GetDocument(module)?.Text ?? string.Empty;

        public TextReader SourceCode(IQualifiedModuleName module) => GetDocument(module).CreateReader();

        public void SubstituteCode(IQualifiedModuleName module, CodeString newCode)
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

        public void SubstituteCode(IQualifiedModuleName module, string newCode)
        {
            var document = GetDocument(module) ?? throw new ArgumentOutOfRangeException(nameof(module));
            document.Text = newCode;
        }

        public void SetSelection(IQualifiedModuleName module, Selection selection)
        {
            //var document = _documentProvider.GetDocument(module);
            
            //var startOffset = document.GetOffset(selection.StartLine, selection.StartColumn);
            //var endOffset = document.GetOffset(selection.EndLine, selection.EndColumn);

            // TODO set document selection
        }

        public CodeString GetCurrentLogicalLine(IQualifiedModuleName module)
        {
            throw new NotImplementedException();
        }

        public int GetContentHash(IQualifiedModuleName module)
        {
            throw new NotImplementedException();
        }
    }
}