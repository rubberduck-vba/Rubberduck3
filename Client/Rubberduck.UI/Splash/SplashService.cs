using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Model.Abstract;
using Rubberduck.SettingsProvider;
using Rubberduck.UI.Services;

namespace Rubberduck.UI.Splash
{
    public class SplashService : WindowService<SplashWindow, ISplashViewModel>, IStatusUpdate
    {
        public SplashService(ILogger<SplashService> logger, RubberduckSettingsProvider settings, ISplashViewModel viewModel)
            : base(logger, settings, viewModel)
        {
        }

        public string Status => Model.Status ?? string.Empty;
        public void UpdateStatus(string status)
        {
            Model.UpdateStatus(status);
            LogTrace("Updated splash status.", status);
        }

        protected override bool PreconditionCheck() => Settings.GeneralSettings.ShowSplash;

        protected override SplashWindow CreateWindow(ISplashViewModel model) => new(model) { Height = 380, Width = 340 };
    }
}
