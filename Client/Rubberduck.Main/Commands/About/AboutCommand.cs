using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.UI.About;
using Rubberduck.UI.Command;
using Rubberduck.Unmanaged.Abstract;
using Rubberduck.VBEditor.UI.OfficeMenus.RubberduckMenu;
using System.Threading.Tasks;

namespace Rubberduck.Main.About
{
    public class AboutCommand : ComCommandBase, IAboutCommand
    {
        private readonly AboutService _service;

        public AboutCommand(ILogger<AboutCommand> logger, ISettingsProvider<RubberduckSettings> settingsProvider, IVbeEvents vbeEvents, AboutService service)
            : base(logger, settingsProvider, vbeEvents)
        {
            _service = service;
        }

        protected async override Task OnExecuteAsync(object? parameter)
        {
            _service.Show();
            await Task.CompletedTask;
        }
    }
}
