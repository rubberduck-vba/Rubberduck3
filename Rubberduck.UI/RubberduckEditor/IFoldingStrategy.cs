using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Folding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Rubberduck.UI.RubberduckEditor
{
    public interface IFoldingStrategy
    {
        void UpdateFoldings(FoldingManager manager, TextDocument document);
    }

    public interface IBlockCompletionStrategy
    {
        bool CanComplete(string text);
        void Complete(Caret caret, string text, TextDocument document);
    }

    public class BlockCompletionStrategy : IBlockCompletionStrategy
    {
        public BlockCompletionStrategy(CodeBlockInfo blockInfo)
        {
            Info = blockInfo;
        }

        protected CodeBlockInfo Info { get; }

        public bool CanComplete(string text)
        {
            return text.TrimStart().StartsWith(Info.StartToken + " ", StringComparison.InvariantCultureIgnoreCase);
        }

        public void Complete(Caret caret, string text, TextDocument document)
        {
            // TODO implement indentation strategy / use indenter settings
            var position = caret.Offset;
            var autocomplete = Info.StartLineCompletion + Environment.NewLine + "    " + Environment.NewLine + Info.EndToken + Environment.NewLine;
            document.Insert(position, autocomplete);
            caret.Offset = position;
        }
    }

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
