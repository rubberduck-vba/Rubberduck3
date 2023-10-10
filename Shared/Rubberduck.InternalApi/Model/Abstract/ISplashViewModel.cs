using System.ComponentModel;

namespace Rubberduck.InternalApi.Model.Abstract
{
    public interface ISplashViewModel : IStatusUpdate, INotifyPropertyChanged
    {
        string Title { get; }
        string Version { get; }
    }
}
