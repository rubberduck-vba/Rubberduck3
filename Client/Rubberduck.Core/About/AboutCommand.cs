using Rubberduck.Interaction.MessageBox;
using Rubberduck.UI;
using Rubberduck.UI.Command;
using Rubberduck.UI.WinForms.Dialogs;
using Rubberduck.VBEditor.UI.OfficeMenus.RubberduckMenu;
using Rubberduck.VersionCheck;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Rubberduck.Core.About
{
    /// <summary>
    /// A command that displays the About window.
    /// </summary>
    [ComVisible(false)]
    public class AboutCommand : CommandBase, IAboutCommand
    {
        public AboutCommand(IVersionCheckService versionService, IWebNavigator web, IMessageBox messageBox)
        {
            _versionService = versionService;
            _web = web;
            _messageBox = messageBox;
        }

        private readonly IVersionCheckService _versionService;
        private readonly IWebNavigator _web;
        private readonly IMessageBox _messageBox;

        protected async override Task OnExecuteAsync(object parameter)
        {
            var vm = new AboutControlViewModel(_versionService, _web, _messageBox);
            using (var window = new AboutDialog(vm))
            {
                window.ShowDialog();
            }
            await Task.CompletedTask;
        }
    }
}
