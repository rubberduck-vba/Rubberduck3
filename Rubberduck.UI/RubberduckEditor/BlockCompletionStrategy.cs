using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using System;
using System.Linq;

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
            var indent = text.TakeWhile(c => char.IsWhiteSpace(c)).Count();
            var newIndent = new string(' ', 4);

            var position = caret.Offset;
            var autocomplete = Info.StartLineCompletion + Environment.NewLine + new string(' ', indent) + newIndent + Environment.NewLine + Info.EndToken + Environment.NewLine;

            document.Replace(document.GetLineByOffset(caret.Offset).Offset, Info.StartToken.Length, Info.StartToken);
            document.Insert(position, autocomplete);
            caret.Offset = position;
        }
    }
}
