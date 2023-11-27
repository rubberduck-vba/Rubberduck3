using System.Threading.Tasks;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.UI.Command.Abstract;
using Rubberduck.UI.Services;
using Rubberduck.UI.Services.Settings;

namespace Rubberduck.UI.Command.SharedHandlers
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
            SettingsWindowViewModel vm;
            if (parameter is string key)
            {
                vm = _settingsDialog.ShowDialog(key);
            }
            else
            {
                vm = _settingsDialog.ShowDialog();
            }
            var model = (RubberduckSettings)vm.Settings.ToSetting();
            Service.SettingsProvider.Write(model);
        }
    }
}
