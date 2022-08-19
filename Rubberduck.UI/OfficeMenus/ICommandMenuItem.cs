using Rubberduck.UI.Command;
using Rubberduck.VBEditor.SafeComWrappers.Office;
using System.Drawing;

namespace Rubberduck.UI.OfficeMenus
{
    public interface ICommandMenuItem : IMenuItem
    {
        bool EvaluateCanExecute(object parameter);
        bool HiddenWhenDisabled { get; }
        bool IsVisible { get; }
        ButtonStyle ButtonStyle { get; }
        IMenuCommand Command { get; }
        Image Image { get; }
        Image Mask { get; }
    }
}