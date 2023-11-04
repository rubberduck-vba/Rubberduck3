using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.UI.Command;
using Rubberduck.UI.Settings;
using Rubberduck.Unmanaged.Abstract;
using Rubberduck.VBEditor.UI.OfficeMenus;
using System.Threading.Tasks;

namespace Rubberduck.Main.Settings
{
    public class SettingsCommand : ComCommandBase, ISettingsCommand
    {
        private readonly ISettingsDialogService _service;

        public SettingsCommand(ILogger<SettingsCommand> logger, ISettingsProvider<RubberduckSettings> settingsProvider, IVbeEvents vbeEvents, ISettingsDialogService service)
            : base(logger, settingsProvider, vbeEvents)
        {
            _service = service;
        }

        protected async override Task OnExecuteAsync(object? parameter)
        {
            _service.ShowDialog();
            await Task.CompletedTask;
        }
    }
}
