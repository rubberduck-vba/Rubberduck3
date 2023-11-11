using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.UI.Services;

namespace Rubberduck.UI.About
{
    public class AboutService : WindowService<AboutWindow, IAboutWindowViewModel>
    {
        public AboutService(ILogger<WindowService<AboutWindow, IAboutWindowViewModel>> logger, RubberduckSettingsProvider settings, IAboutWindowViewModel viewModel)
            : base(logger, settings, viewModel)
        {
        }

        protected override AboutWindow CreateWindow(IAboutWindowViewModel model) => new(model);
    }
}
