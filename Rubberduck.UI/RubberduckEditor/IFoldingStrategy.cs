using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using Rubberduck.UI.Abstract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Rubberduck.UI.RubberduckEditor
{
    public interface IFoldingStrategy
    {
        IEnumerable<(int StartOffset, int EndOffset, MemberType MemberType, string Name)> UpdateFoldings(FoldingManager manager, TextDocument document);
    }
}
