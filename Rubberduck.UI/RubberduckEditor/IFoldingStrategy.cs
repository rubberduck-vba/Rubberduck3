using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Rubberduck.UI.RubberduckEditor
{
    public interface IFoldingStrategy
    {
        void UpdateFoldings(FoldingManager manager, TextDocument document);
        event EventHandler<ScopeEventArgs> ScopeCreated;
        event EventHandler<ScopeEventArgs> ScopeRemoved;
    }
}
