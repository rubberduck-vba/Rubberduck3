using Rubberduck.InternalApi.Model.Abstract;
using Rubberduck.SettingsProvider;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.UI.Abstract;
using Rubberduck.UI.Xaml.Splash;

namespace Rubberduck.UI.Splash
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
        protected override SplashWindow? CreateWindow(ISplashViewModel model) => CanShowSplash ? new SplashWindow(model) : null;
    }
}
