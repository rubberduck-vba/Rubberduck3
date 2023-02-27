using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace Rubberduck.UI.RubberduckEditor
{
    public interface IBlockCompletionStrategy
    {
        bool CanComplete(string text);
        void Complete(Caret caret, string text, TextDocument document);
    }
}
