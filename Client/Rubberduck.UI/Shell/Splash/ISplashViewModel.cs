using Rubberduck.InternalApi.Model.Abstract;
using System.ComponentModel;

namespace Rubberduck.UI.Shell.Splash
{
    public interface ISplashViewModel : IStatusUpdate, INotifyPropertyChanged
    {
        string Title { get; }
        string Version { get; }
    }
}
