using ICSharpCode.AvalonEdit.Document;

namespace Rubberduck.UI.Xaml.Controls
{
    public interface IEnterKeyStrategy
    {
        bool IsActive { get; set; }
        bool HandleEvent(TextDocument document, ref int caretOffset);
    }
}
