using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.UI.Services.Abstract;
using Rubberduck.UI.Shell.About;

namespace Rubberduck.UI.Shared.About
{
    public class AboutService : WindowService<AboutWindow, IAboutWindowViewModel>
    {
        public AboutService(ILogger<WindowService<AboutWindow, IAboutWindowViewModel>> logger, RubberduckSettingsProvider settings, IAboutWindowViewModel viewModel, PerformanceRecordAggregator performance)
            : base(logger, settings, viewModel, performance)
        {
        }

        protected override AboutWindow CreateWindow(IAboutWindowViewModel model) => new(model);
    }
}
