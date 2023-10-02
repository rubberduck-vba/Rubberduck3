using Rubberduck.Unmanaged.UIContext;

namespace Rubberduck.VBEditor.UI.OfficeMenus.RubberduckMenu
{
    public interface IRubberduckMenu 
    {
        void Localize();
        void Initialize();
    }

    public class RubberduckParentMenu : ParentMenuItemBase, IRubberduckMenu
    {
        public RubberduckParentMenu(IUiDispatcher dispatcher) 
            : base(dispatcher)
        {
        }

        public override string ResourceKey { get; } = "RubberduckMenu";
    }
}
