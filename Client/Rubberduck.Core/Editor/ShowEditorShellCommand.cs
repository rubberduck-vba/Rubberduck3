using Rubberduck.UI;
using Rubberduck.UI.Command;
using Rubberduck.Unmanaged.Abstract;
using Rubberduck.VBEditor.UI.OfficeMenus;
using System.Threading.Tasks;

namespace Rubberduck.Core.Editor
{
    class ShowEditorShellCommand : ComCommandBase, IShowEditorShellCommand
    {
        private readonly IPresenter _presenter;

        public ShowEditorShellCommand(IPresenter presenter, IVbeEvents vbeEvents)
            : base(vbeEvents)
        {
            _presenter = presenter;
        }

        protected async override Task OnExecuteAsync(object? parameter)
        {
            _presenter.Show();
            await Task.CompletedTask;
        }
    }
}
