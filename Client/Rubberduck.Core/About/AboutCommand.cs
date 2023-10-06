using Microsoft.Extensions.Logging;
using Rubberduck.Interaction.MessageBox;
using Rubberduck.UI;
using Rubberduck.UI.Command;
using Rubberduck.UI.WinForms.Dialogs;
using Rubberduck.Unmanaged.Abstract;
using Rubberduck.VBEditor.UI.OfficeMenus.RubberduckMenu;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Rubberduck.Core.About
{
    /// <summary>
    /// A command that displays the About window.
    /// </summary>
    [ComVisible(false)]
    public class AboutCommand : ComCommandBase, IAboutCommand
    {
        private readonly IWebNavigator _web;
        private readonly IMessageBox _messageBox;
        private readonly Version _version;

        public AboutCommand(ILogger<AboutCommand> logger, IVbeEvents vbeEvents, IWebNavigator web, IMessageBox messageBox, Version version)
            : base(logger, vbeEvents)
        {
            _web = web;
            _messageBox = messageBox;
            _version = version;
        }

        protected async override Task OnExecuteAsync(object? parameter)
        {
            var vm = new AboutControlViewModel(Logger, _web, _messageBox, _version);
            using (var window = new AboutDialog(vm))
            {
                window.ShowDialog();
            }
            await Task.CompletedTask;
        }
    }
}
