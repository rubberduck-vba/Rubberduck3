using Microsoft.Extensions.Logging;
using Rubberduck.SettingsProvider;

namespace Rubberduck.UI.About
{
    public class AboutService : WindowService<AboutWindow, IAboutWindowViewModel>
    {
        public AboutService(ILogger<WindowService<AboutWindow, IAboutWindowViewModel>> logger, IRubberduckSettingsProvider settings, IAboutWindowViewModel viewModel)
            : base(logger, settings, viewModel)
        {
        }

        protected override AboutWindow CreateWindow(IAboutWindowViewModel model) => new(model);
    }
}
