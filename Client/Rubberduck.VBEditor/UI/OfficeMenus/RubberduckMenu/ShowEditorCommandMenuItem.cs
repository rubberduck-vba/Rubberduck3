using Rubberduck.VBEditor.UI.OfficeMenus.RubberduckMenu;

namespace Rubberduck.VBEditor.UI.OfficeMenus
{
    public interface IShowEditorShellCommand : IMenuCommand { }

    public class ShowEditorShellCommandMenuItem : CommandMenuItemBase
    {
        public ShowEditorShellCommandMenuItem(IShowEditorShellCommand command) : base(command)
        {
        }

        public override string ResourceKey => "RubberduckMenu_ShowEditor";
        public override bool BeginGroup => false;
        public override int DisplayOrder => (int)RubberduckParentMenu.ItemDisplayOrder.ShowEditor;

        public override bool EvaluateCanExecute(object parameter) => true;
    }
}
