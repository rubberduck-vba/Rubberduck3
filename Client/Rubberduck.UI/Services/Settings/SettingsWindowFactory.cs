using Rubberduck.UI.Shared.Settings;
using Rubberduck.UI.Windows;

namespace Rubberduck.UI.Services.Settings
{
    public class SettingsWindowFactory : IWindowFactory<SettingsWindow, SettingsWindowViewModel>
    {
        public SettingsWindow Create(SettingsWindowViewModel model) => new(model);
    }
}
