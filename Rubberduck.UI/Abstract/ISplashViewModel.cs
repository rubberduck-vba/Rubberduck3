using System.ComponentModel;

namespace Rubberduck.UI.Abstract
{
    public interface ISplashViewModel : IStatusUpdate, INotifyPropertyChanged
    {
        string Version { get; }
    }
}
