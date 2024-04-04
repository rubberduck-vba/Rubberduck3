namespace Rubberduck.VBEditor.UI.OfficeMenus
{
    public interface ISettingsCommand : IMenuCommand { }

    public class SettingsCommandMenuItem : CommandMenuItemBase
    {
        public SettingsCommandMenuItem(ISettingsCommand command) : base(command)
        {
        }

        public override string ResourceKey => "RubberduckMenu_Settings";
        public override bool EvaluateCanExecute(object? parameter) => true;
    }
}
