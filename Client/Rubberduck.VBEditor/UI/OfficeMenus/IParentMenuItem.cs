using Rubberduck.Unmanaged.Abstract.SafeComWrappers.Office;

namespace Rubberduck.VBEditor.UI.OfficeMenus
{
    public interface IParentMenuItem : IMenuItem, IAppMenu
    {
        ICommandBarControls Parent { get; set; }
        ICommandBarPopup Item { get; }
        void RemoveMenu();
    }
}
