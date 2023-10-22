using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.UI.Command;
using Rubberduck.Unmanaged.Abstract;
using Rubberduck.VBEditor.UI.OfficeMenus.RubberduckMenu;
using System.Threading.Tasks;

namespace Rubberduck.Core.Editor
{
    public class AboutCommand : ComCommandBase, IAboutCommand
    {
        public AboutCommand(ILogger logger, ISettingsProvider<RubberduckSettings> settingsProvider, IVbeEvents vbeEvents) 
            : base(logger, settingsProvider, vbeEvents)
        {
        }

        protected async override Task OnExecuteAsync(object? parameter)
        {
            throw new System.NotImplementedException();
        }
    }
}
