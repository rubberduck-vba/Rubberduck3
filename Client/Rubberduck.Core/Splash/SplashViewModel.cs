using Rubberduck.UI;
using Rubberduck.UI.Abstract;
using Rubberduck.VersionCheck;
namespace Rubberduck.Core.Splash
{
    public class SplashViewModel : ViewModelBase, ISplashViewModel
    {
        public SplashViewModel(string version)
        {
            Version = version;
        }

        public string Version { get; }

        private string _status = "Initializing..."; // TODO move to .resx
        public string CurrentStatus 
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

        public void UpdateStatus(string status)
        {
            CurrentStatus = status;
        }
    }
}
