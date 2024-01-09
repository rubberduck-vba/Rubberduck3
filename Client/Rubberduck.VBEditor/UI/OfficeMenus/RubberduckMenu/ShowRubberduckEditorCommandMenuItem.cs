using System;

namespace Rubberduck.VBEditor.UI.OfficeMenus
{
    public interface IShowRubberduckEditorCommand : IMenuCommand 
    {
    }

    public class ShowRubberduckEditorCommandMenuItem : CommandMenuItemBase
    {
        public ShowRubberduckEditorCommandMenuItem(IShowRubberduckEditorCommand command) : base(command)
        {
        }

        public override string ResourceKey => "RubberduckMenu_ShowEditor";
        public override bool EvaluateCanExecute(object? parameter) => true;
    }

    public interface INewWorkspaceCommand : IMenuCommand
    {
    }

    public class NewWorkspaceCommandMenuItem : CommandMenuItemBase
    {
        public NewWorkspaceCommandMenuItem(INewWorkspaceCommand command) : base(command)
        {
        }

        public override string ResourceKey => "RubberduckMenu_NewWorkspace";
        public override bool EvaluateCanExecute(object? parameter) => true;
    }

    public interface IShowApplicationTipsCommand : IMenuCommand
    {
    }

    public class ShowAplicationTipsCommandMenuItem : CommandMenuItemBase
    {
        public ShowAplicationTipsCommandMenuItem(IMenuCommand command) 
            : base(command)
        {
        }

        public override string ResourceKey => "RubberduckMenu_ApplicationTips";
        public override bool EvaluateCanExecute(object? parameter) => true;
    }
}
