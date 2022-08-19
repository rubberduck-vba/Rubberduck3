using Rubberduck.UI;
using Rubberduck.UI.Splash;
using Rubberduck.VersionCheck;
namespace Rubberduck.Core.Splash
{
    public class SplashViewModel : ViewModelBase, ISplashViewModel
    {
        public SplashViewModel(IVersionCheckService versionCheck)
        {
            Version = $"v{versionCheck.CurrentVersion}";
        }

        public string Version { get; }

        private string _status = "Initializing..."; // TODO move to .resx
        public string InitializationStatus 
        {
            get => _status;
            private set 
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged();
                }
            }
        }

        public void UpdateInitializationStatus(string status)
        {
            InitializationStatus = status;
        }
    }
}
