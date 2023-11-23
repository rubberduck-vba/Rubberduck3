using System.Threading.Tasks;
using Rubberduck.UI.Command.Abstract;
using Rubberduck.UI.Services;
using Rubberduck.UI.Settings;

namespace Rubberduck.UI.Command
{
    public class ShowRubberduckSettingsCommand : CommandBase
    {
        private readonly ISettingsDialogService _settingsDialog;

        public ShowRubberduckSettingsCommand(UIServiceHelper service, ISettingsDialogService settingsDialog) 
            : base(service)
        {
            _settingsDialog = settingsDialog;
        }

        protected async override Task OnExecuteAsync(object? parameter)
        {
            if (parameter is string key)
            {
                _settingsDialog.ShowDialog(key);
            }
            else
            {
                _settingsDialog.ShowDialog();
            }
        }
    }
}
