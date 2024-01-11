using Rubberduck.UI.Command;
using Rubberduck.UI.Services;
using Rubberduck.UI.Shell.About;
using Rubberduck.Unmanaged.Abstract;
using Rubberduck.VBEditor.UI.OfficeMenus.RubberduckMenu;
using System.Threading.Tasks;

namespace Rubberduck.Main.About
{
    public class AboutCommand : ComCommandBase, IAboutCommand
    {
        private readonly AboutService _service;

        public AboutCommand(UIServiceHelper service, IVbeEvents vbeEvents, AboutService aboutService)
            : base(service, vbeEvents)
        {
            _service = aboutService;
        }

        protected async override Task OnExecuteAsync(object? parameter)
        {
            _service.Show();
            await Task.CompletedTask;
        }
    }
}
