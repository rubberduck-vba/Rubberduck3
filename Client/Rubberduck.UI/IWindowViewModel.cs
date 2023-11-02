using System.ComponentModel;

namespace Rubberduck.UI
{
    public interface IWindowViewModel : INotifyPropertyChanged
    {
        string Title { get; }
    }
}
