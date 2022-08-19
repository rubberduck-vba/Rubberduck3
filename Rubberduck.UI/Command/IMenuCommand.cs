using System.Windows.Input;

namespace Rubberduck.UI.Command
{
    public interface IMenuCommand : ICommand
    {
        string ShortcutText { get; set; }
    }
}
