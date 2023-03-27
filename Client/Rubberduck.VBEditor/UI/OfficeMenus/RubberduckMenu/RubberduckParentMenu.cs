using System.Collections.Generic;
using Rubberduck.InternalApi.UIContext;

namespace Rubberduck.VBEditor.UI.OfficeMenus.RubberduckMenu
{
    public class RubberduckParentMenu : ParentMenuItemBase
    {
        public RubberduckParentMenu(IUiDispatcher dispatcher) 
            : base(dispatcher)
        {
        }

        public override int? BeforeIndex { get; set; }

        public override string ResourceKey { get; } = "RubberduckMenu";

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
