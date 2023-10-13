using Rubberduck.InternalApi.Model.Abstract;
using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model;

namespace Rubberduck.Editor.Splash
{
    public class SplashService : WindowService<SplashWindow, ISplashViewModel>, IStatusUpdate
    {
        private readonly ISettingsProvider<RubberduckSettings> _settings;

        public SplashService(ISplashViewModel viewModel, ISettingsProvider<RubberduckSettings> settings)
            : base(viewModel)
        {
            _settings = settings;
        }

        public string Status => Model.Status ?? string.Empty;
        public void UpdateStatus(string status) => Model.UpdateStatus(status);

        public bool CanShowSplash => _settings.Settings.ShowSplash;
        protected override SplashWindow CreateWindow(ISplashViewModel model) => new(model);
    }
}
