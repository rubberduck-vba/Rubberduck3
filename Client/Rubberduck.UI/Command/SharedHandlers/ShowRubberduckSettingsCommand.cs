using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Settings.Model;
using Rubberduck.Resources.v3;
using Rubberduck.UI.Command.Abstract;
using Rubberduck.UI.Services;
using Rubberduck.UI.Services.Settings;
using Rubberduck.UI.Shared.Message;

namespace Rubberduck.UI.Command.SharedHandlers
{
    public class ShowRubberduckSettingsCommand : CommandBase
    {
        private readonly ISettingsDialogService _settingsDialog;
        private readonly IMessageService _message;

        public ShowRubberduckSettingsCommand(UIServiceHelper service, 
            ISettingsDialogService settingsDialog,
            IMessageService message)
            : base(service)
        {
            _settingsDialog = settingsDialog;
            _message = message;
        }

        protected async override Task OnExecuteAsync(object? parameter)
        {
            SettingsWindowViewModel? vm;
            if (parameter is string key)
            {
                vm = _settingsDialog.ShowDialog(key);
            }
            else
            {
                if (!_settingsDialog.ShowDialog(out vm) 
                    || vm?.SelectedAction is null 
                    || vm.SelectedAction == MessageAction.CancelAction)
                {
                    return;
                }
            }

            if (vm?.SelectedAction?.IsDefaultAction == true)
            {
                var oldSettings = Service.SettingsProvider.Settings;
                var newSettings = (RubberduckSettings)vm.Settings.ToSetting();
                var diff = oldSettings.Diff(newSettings);

                if (diff.Any())
                {
                    Service.SettingsProvider.Write(newSettings);

                    var details = new StringBuilder();
                    foreach (var item in diff)
                    {
                        var itemTitle = SettingsUI.ResourceManager.GetString($"{item.Key}_Title");
                        if (item.SettingDataType == SettingDataType.ListSetting)
                        {
                            var nbReferenceValues = item.ReferenceValue is null ? 0 : ((object[])item.ReferenceValue.Value!).Length;
                            var nbComparableValues = item.ComparableValue is null ? 0 : ((object[])item.ComparableValue.Value!).Length;
                            details.AppendLine($"•{itemTitle}: {nbReferenceValues} items -> {nbComparableValues} items");
                        }
                        else
                        {
                            details.AppendLine($"•{itemTitle}: {item.ReferenceValue?.Value ?? "(added)"} -> {item.ComparableValue?.Value ?? "(removed)"}");
                        }
                    }

                    var msgKey = "SettingsSaved";
                    var message = SettingsUI.ResourceManager.GetString($"{msgKey}_Message") ?? $"[missing key:{msgKey}_Message]";
                    var title = SettingsUI.ResourceManager.GetString($"{msgKey}_Title") ?? $"[missing key:{msgKey}_Title]";
                    _message.ShowMessage(new() { Key = msgKey, Level = LogLevel.Information, Title = title, Message = message, Verbose = $"Modified settings:\n{details}" });
                }
            }
        }
    }
}
