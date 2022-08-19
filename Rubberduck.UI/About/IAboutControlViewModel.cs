using Rubberduck.UI.Command;

namespace Rubberduck.UI.About
{
    public interface IAboutControlViewModel
    {
        string Version { get; }
        string OperatingSystem { get; }
        string HostProduct { get; }
        string HostVersion { get; }
        string HostExecutable { get; }
        string AboutCopyright { get; }

        CommandBase UriCommand { get; }
        CommandBase ViewLogCommand { get; }
    }
}
