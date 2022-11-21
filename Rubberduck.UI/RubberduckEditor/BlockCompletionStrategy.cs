using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using System;

namespace Rubberduck.UI.RubberduckEditor
{
    public class BlockCompletionStrategy : IBlockCompletionStrategy
    {
        public BlockCompletionStrategy(FoldingCodeBlockInfo blockInfo)
        {
            Info = blockInfo;
        }

        protected FoldingCodeBlockInfo Info { get; }

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
}
