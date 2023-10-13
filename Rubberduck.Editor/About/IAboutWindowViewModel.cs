using System.ComponentModel;
using System.Windows.Input;

namespace Rubberduck.Editor.About
{
    public interface IAboutWindowViewModel : INotifyPropertyChanged
    {
        string Version { get; }
        string OperatingSystem { get; }
        string HostProduct { get; }
        string HostVersion { get; }
        string HostExecutable { get; }
        string AboutCopyright { get; }

        ICommand UriCommand { get; }
        ICommand ViewLogCommand { get; }

        void CopyVersionInfo();

        string Document { get; }
    }
}
