using System.Windows.Input;

namespace Rubberduck.VBEditor.UI.OfficeMenus
{
    public interface IMenuCommand : ICommand
    {
        string ShortcutText { get; set; }
    }
}
