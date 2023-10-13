using Microsoft.Extensions.Logging;
using Rubberduck.SettingsProvider;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.UI.Command;
using Rubberduck.Unmanaged.Abstract;
using Rubberduck.VBEditor.UI.OfficeMenus.RubberduckMenu;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Rubberduck.Core.About
{
    /*

    /// <summary>
    /// A command that displays the About window.
    /// </summary>
    [ComVisible(false)]
    public class AboutCommand : ComCommandBase, IAboutCommand
    {
        private AboutService _service;

        public AboutCommand(ILogger<AboutCommand> logger, ISettingsProvider<RubberduckSettings> settingsProvider, IVbeEvents vbeEvents, AboutService service)
            : base(logger, settingsProvider, vbeEvents)
        {
            _service = service;
        }

        protected override Task OnExecuteAsync(object? parameter)
        {
            _service.Show();
            return Task.CompletedTask;
        }
    }
    */
}
