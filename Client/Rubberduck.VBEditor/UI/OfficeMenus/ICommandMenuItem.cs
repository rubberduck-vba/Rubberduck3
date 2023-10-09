using Rubberduck.UI.Command;
using Rubberduck.Unmanaged.Abstract.SafeComWrappers.Office.Enums;
using System.Drawing;

namespace Rubberduck.VBEditor.UI.OfficeMenus
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