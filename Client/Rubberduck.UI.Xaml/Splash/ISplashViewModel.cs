using System.ComponentModel;

namespace Rubberduck.UI.Xaml.Splash
{
    public interface ISplashViewModel : IStatusUpdate, INotifyPropertyChanged
    {
        string Title { get; }
        string Version { get; }
    }
}
