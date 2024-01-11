using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Model.Abstract;
using Rubberduck.SettingsProvider;
using Rubberduck.UI.Services.Abstract;
using Rubberduck.UI.Shell.Splash;

namespace Rubberduck.Editor.Shell.Splash
{
    public class SplashService : WindowService<SplashWindow, ISplashViewModel>, IStatusUpdate
    {
        public SplashService(ILogger<SplashService> logger, RubberduckSettingsProvider settings, ISplashViewModel viewModel, PerformanceRecordAggregator performance)
            : base(logger, settings, viewModel, performance)
        {
        }

        public string Status => Model.Status ?? string.Empty;
        public void UpdateStatus(string status)
        {
            Model.UpdateStatus(status);
            LogTrace("Updated splash status.", status);
        }

        protected override bool PreconditionCheck() => Settings.GeneralSettings.ShowSplash;

        protected override SplashWindow CreateWindow(ISplashViewModel model) => new(model);
    }
}
