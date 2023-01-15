using Rubberduck.VBEditor.SafeComWrappers.Abstract;

namespace Rubberduck.VBEditor.UI.OfficeMenus
{
    public interface IParentMenuItem : IMenuItem, IAppMenu
    {
        ICommandBarControls Parent { get; set; }
        ICommandBarPopup Item { get; }
        void RemoveMenu();
    }
}
