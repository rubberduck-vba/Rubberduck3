using Rubberduck.VBEditor.SafeComWrappers.Abstract;

namespace Rubberduck.UI.OfficeMenus.MenuItems
{
    public interface IParentMenuItem : IMenuItem, IAppMenu
    {
        ICommandBarControls Parent { get; set; }
        ICommandBarPopup Item { get; }
        void RemoveMenu();
    }
}
