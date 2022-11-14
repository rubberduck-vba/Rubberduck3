using RD = Rubberduck.VBEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Document;
using NLog.LayoutRenderers;

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
