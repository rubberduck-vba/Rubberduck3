using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Model.Abstract;
using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.UI;

namespace Rubberduck.Editor.Splash
{
    public class SplashService : WindowService<SplashWindow, ISplashViewModel>, IStatusUpdate
    {
        private readonly ISettingsProvider<RubberduckSettings> _settings;

        public SplashService(ILogger<SplashService> logger, ISplashViewModel viewModel, ISettingsProvider<RubberduckSettings> settings)
            : base(logger, settings, viewModel)
        {
            _settings = settings;
        }

        public string Status => Model.Status ?? string.Empty;
        public void UpdateStatus(string status) => Model.UpdateStatus(status);

        protected override bool PreconditionCheck() => _settings.Settings.GeneralSettings.ShowSplash;

        protected override SplashWindow CreateWindow(ISplashViewModel model) => new(model);
    }
}
