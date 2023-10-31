using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.UI.Command;

namespace Rubberduck.UI.Settings
{

    public class SettingsWindowFactory : IWindowFactory<SettingsWindow, SettingsWindowViewModel>
    {
        public SettingsWindow Create(SettingsWindowViewModel model) => new(model);
    }

    public interface ISettingsDialogService : IDialogService<SettingsWindowViewModel>
    {
    }

    public class SettingsDialogService : DialogService<SettingsWindow, SettingsWindowViewModel>, ISettingsDialogService
    {
        public SettingsDialogService(ILogger logger, 
            IWindowFactory<SettingsWindow, SettingsWindowViewModel> factory, 
            ISettingsProvider<RubberduckSettings> settingsProvider, 
            MessageActionsProvider actionsProvider) 
            : base(logger, factory, settingsProvider, actionsProvider)
        {
        }

        protected override SettingsWindowViewModel CreateViewModel(RubberduckSettings settings, MessageActionsProvider actions)
        {
            return new SettingsWindowViewModel(actions.Close());
        }
    }
}
