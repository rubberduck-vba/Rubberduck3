using System;

namespace Rubberduck.VBEditor.UI.OfficeMenus
{
    public interface IShowRubberduckEditorCommand : IMenuCommand 
    {
        public event EventHandler Executed;
    }

    public class ShowRubberduckEditorCommandMenuItem : CommandMenuItemBase
    {
        public ShowRubberduckEditorCommandMenuItem(IShowRubberduckEditorCommand command) : base(command)
        {
        }

        public override string ResourceKey => "RubberduckMenu_ShowEditor";
        public override bool EvaluateCanExecute(object? parameter) => true;
    }
}
