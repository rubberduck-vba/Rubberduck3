using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubberduck.UI.RubberduckEditor
{
    public interface IFoldingStrategy
    {
        void UpdateFoldings(FoldingManager manager, TextDocument document);
    }
}
