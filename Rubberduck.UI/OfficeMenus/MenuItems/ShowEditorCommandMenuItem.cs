using Rubberduck.UI.Command.MenuItems.ParentMenus;
using Rubberduck.UI.OfficeMenus;

namespace Rubberduck.UI.Command.MenuItems
{
    public interface IShowEditorShellCommand : IMenuCommand { }

    public class ShowEditorShellCommandMenuItem : CommandMenuItemBase
    {
        public ShowEditorShellCommandMenuItem(IShowEditorShellCommand command) : base(command)
        {
        }

        public override string Key => "Rubberduck_Editor";
        public override bool BeginGroup => false;
        public override int DisplayOrder => (int)RubberduckParentMenu.ItemDisplayOrder.ShowEditor;

        public override bool EvaluateCanExecute(object parameter) => true;
    }
}
