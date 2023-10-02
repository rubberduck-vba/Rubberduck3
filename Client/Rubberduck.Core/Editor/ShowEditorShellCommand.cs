using Rubberduck.UI.Command;
using Rubberduck.Unmanaged.Abstract;
using Rubberduck.VBEditor.UI.OfficeMenus;
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

        protected async override Task OnExecuteAsync(object? parameter)
        {
            _presenter?.Show();
            await Task.CompletedTask;
        }
    }
}
