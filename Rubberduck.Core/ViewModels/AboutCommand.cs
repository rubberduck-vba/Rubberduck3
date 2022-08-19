using System.Runtime.InteropServices;
using Rubberduck.UI;
using Rubberduck.UI.About;
using Rubberduck.UI.Command;
using Rubberduck.UI.Command.MenuItems;
using Rubberduck.VersionCheck;

namespace Rubberduck.Core.Command
{
    /// <summary>
    /// A command that displays the About window.
    /// </summary>
    [ComVisible(false)]
    public class AboutCommand : CommandBase, IAboutCommand
    {
        private readonly IVersionCheckService _version;
        private readonly IWebNavigator _web;

        public AboutCommand(IVersionCheckService version, IWebNavigator web)
        {
            _version = version;
            _web = web;
        }

        protected override void OnExecute(object parameter)
        {
            var viewModel = new AboutControlViewModel(_version, _web);
            using (var window = new AboutDialog(viewModel))
            {
                window.ShowDialog();
            }
        }
    }
}
