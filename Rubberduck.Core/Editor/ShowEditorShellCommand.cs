using Rubberduck.UI.Command;
using Rubberduck.UI.Command.MenuItems;
using Rubberduck.VBEditor.Events;

namespace Rubberduck.Core.Editor
{
    public class ShowEditorShellCommand : ComCommandBase, IShowEditorShellCommand
    {
        private readonly EditorShellDockablePresenter _presenter;

        public ShowEditorShellCommand(EditorShellDockablePresenter presenter, IVbeEvents vbeEvents)
            : base(vbeEvents)
        {
            _presenter = presenter;
        }

        protected override void OnExecute(object parameter)
        {
            _presenter?.Show();
        }
    }
}
