using System.ComponentModel;

namespace Rubberduck.InternalApi.Model.Abstract
{
    public interface IMessageWindowViewModel : INotifyPropertyChanged
    {
        string Title { get; }
        string Message { get; }
        string Verbose { get; }

        MessageAction[] Actions { get; }
    }
}
