using System.ComponentModel;

namespace Rubberduck.UI.Splash
{
    public interface ISplashViewModel : INotifyPropertyChanged
    {
        string Version { get; }
        string InitializationStatus { get; }

        void UpdateInitializationStatus(string status);
    }
}
