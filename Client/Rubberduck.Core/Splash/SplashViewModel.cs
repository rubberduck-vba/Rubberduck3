using Rubberduck.UI;
using Rubberduck.UI.Abstract;

namespace Rubberduck.Core.Splash
{
    public class SplashViewModel : ViewModelBase, ISplashViewModel
    {
        public SplashViewModel(string version)
        {
            Version = version;
        }

        public string Version { get; }

        private string _status = "Long live the cucumber!";
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
