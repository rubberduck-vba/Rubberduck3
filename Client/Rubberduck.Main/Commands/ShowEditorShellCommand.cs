using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model;
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

        public ShowEditorShellCommand(ILogger<ShowEditorShellCommand> logger, ISettingsProvider<RubberduckSettings> settingsProvider, IVbeEvents vbeEvents, IPresenter presenter)
            : base(logger, settingsProvider, vbeEvents)
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