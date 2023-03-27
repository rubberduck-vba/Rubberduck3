namespace Rubberduck.VBEditor.UI.OfficeMenus.RubberduckMenu
{
    public interface IAboutCommand : IMenuCommand { }

    public class AboutCommandMenuItem : CommandMenuItemBase
    {
        public AboutCommandMenuItem(IAboutCommand command) : base(command)
        {
        }

        public override string ResourceKey => "RubberduckMenu_About";
        public override bool BeginGroup => true;
        public override int DisplayOrder => (int)RubberduckParentMenu.ItemDisplayOrder.About;

        public override bool EvaluateCanExecute(object parameter) => true;
    }
}
