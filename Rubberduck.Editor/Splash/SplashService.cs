using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Model.Abstract;
using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.UI.Services;

namespace Rubberduck.Editor.Splash
{
    public class SplashService : WindowService<SplashWindow, ISplashViewModel>, IStatusUpdate
    {
        public SplashService(ILogger<SplashService> logger, ISplashViewModel viewModel, RubberduckSettingsProvider settings)
            : base(logger, settings, viewModel)
        {
        }

        public string Status => Model.Status ?? string.Empty;
        public void UpdateStatus(string status) => Model.UpdateStatus(status);

        protected override bool PreconditionCheck() => Settings.GeneralSettings.ShowSplash;

        protected override SplashWindow CreateWindow(ISplashViewModel model) => new(model);
    }
}
