using System.Collections.Generic;
using Rubberduck.InternalApi.UIContext;
using Rubberduck.UI.OfficeMenus;
using Rubberduck.UI.OfficeMenus.MenuItems;

namespace Rubberduck.UI.Command.MenuItems.ParentMenus
{
    public class RubberduckParentMenu : ParentMenuItemBase
    {
        public RubberduckParentMenu(IEnumerable<IMenuItem> items, int beforeIndex, IUiDispatcher dispatcher) 
            : base(dispatcher, "RubberduckMenu", items, beforeIndex)
        {
        }

        public enum ItemDisplayOrder
        {
            ShowEditor,
            Refresh,
            UnitTesting,
            Refactorings,
            Navigate,
            Tools,
            CodeInspections,
            Settings,
            About,
        }
    }
}
