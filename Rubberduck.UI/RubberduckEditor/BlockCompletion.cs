using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Folding;
using System.Collections.Generic;
using System.Linq;

namespace Rubberduck.UI.RubberduckEditor
{
    public class BlockCompletion
    {
        private readonly IEnumerable<IBlockCompletionStrategy> _strategies;

        public BlockCompletion(IEnumerable<IBlockCompletionStrategy> strategies)
        {
            _strategies = strategies ?? Enumerable.Empty<IBlockCompletionStrategy>();
        }

        public bool CanComplete(Caret caret, TextDocument document, FoldingManager foldingManager, out IBlockCompletionStrategy completionStrategy, out string text)
        {
            completionStrategy = null;

            var line = document.Lines[caret.Line - 1];
            text = document.GetText(line.Offset, line.Length);

            // we've already traversed the document to find completed blocks;
            // if there's a fold at line.Offset, the block is already completed.
            var folds = foldingManager.GetFoldingsAt(line.Offset);

            if (!folds.Any())
            {
                var lineText = text;
                completionStrategy = _strategies.FirstOrDefault(e => e.CanComplete(lineText));
            }

            return completionStrategy != null;
        }
    }
}
