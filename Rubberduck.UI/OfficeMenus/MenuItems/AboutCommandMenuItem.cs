using Rubberduck.UI.Command.MenuItems.ParentMenus;
using Rubberduck.UI.OfficeMenus;

namespace Rubberduck.UI.Command.MenuItems
{
    public interface IAboutCommand : IMenuCommand { }

    public class AboutCommandMenuItem : CommandMenuItemBase
    {
        public AboutCommandMenuItem(IAboutCommand command) : base(command)
        {
        }

        public override string Key => "RubberduckMenu_About";
        public override bool BeginGroup => true;
        public override int DisplayOrder => (int)RubberduckParentMenu.ItemDisplayOrder.About;

        public override bool EvaluateCanExecute(object parameter) => true;
    }
}
