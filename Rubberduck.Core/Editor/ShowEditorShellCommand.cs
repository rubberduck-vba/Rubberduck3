using Rubberduck.UI;
using Rubberduck.UI.Command;
using Rubberduck.UI.Command.MenuItems;
using Rubberduck.VBEditor.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
