using RD = Rubberduck.VBEditor;
using ICSharpCode.AvalonEdit.Document;

namespace Rubberduck.UI
{
    public static class SelectionExtensions
    {
        public static (int start, int length) ToTextEditorSelection(this RD.Selection selection, TextDocument document)
        {
            if (selection.IsSingleLine)
            {
                var line = document.GetLineByNumber(selection.StartLine);
                var offset = line.Offset + selection.StartColumn;
                var length = selection.EndColumn - selection.StartColumn + 1;
                return (offset, length);
            }
            else
            {
                var startLine = document.GetLineByNumber(selection.StartLine);
                var endLine = document.GetLineByNumber(selection.EndLine);
                var offset = startLine.Offset + selection.StartColumn;
                var length = endLine.Offset - startLine.Offset + selection.EndColumn - selection.StartColumn;
                return (offset, length);
            }
        }
    }
}
