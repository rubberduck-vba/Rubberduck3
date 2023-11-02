using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.UI.Command;
using Rubberduck.Unmanaged.Abstract;
using Rubberduck.VBEditor.UI.OfficeMenus;
using System.Threading.Tasks;

namespace Rubberduck.Main.Settings
{
    public class SettingsCommand : ComCommandBase, ISettingsCommand
    {
        //private readonly SettingsService _service;

        public SettingsCommand(ILogger<SettingsCommand> logger, ISettingsProvider<RubberduckSettings> settingsProvider, IVbeEvents vbeEvents/*, SettingsService service*/)
            : base(logger, settingsProvider, vbeEvents)
        {
            //_service = service;
        }

        protected async override Task OnExecuteAsync(object? parameter)
        {
            //_service.Show();
            await Task.CompletedTask;
        }
    }
}
