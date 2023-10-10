using Rubberduck.InternalApi.Model.Abstract;
using System.ComponentModel;

namespace Rubberduck.InternalApi.Model.Design
{
    public class SplashDesignViewModel : ISplashViewModel, INotifyPropertyChanged
    {
        public string Title { get; set; } = "Rubberduck";
        public string Version { get; set; } = "v3.0.x (debug)";
        public string Status { get; set; } = "LONG LIVE THE CUCUMBER!";

        public event PropertyChangedEventHandler? PropertyChanged;

        public void UpdateStatus(string status)
        {
            Status = status;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Status)));
        }
    }
}
