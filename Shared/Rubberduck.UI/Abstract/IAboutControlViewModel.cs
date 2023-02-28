using System.Windows.Input;

namespace Rubberduck.UI.Abstract
{
    public interface IAboutControlViewModel
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
    }
}
