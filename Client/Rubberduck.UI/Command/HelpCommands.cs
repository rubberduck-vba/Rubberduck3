using System.Windows.Input;
using System.Windows;

namespace Rubberduck.UI.Command
{
    public static class HelpCommands
    {
        public static RoutedCommand ShowReleaseNotesCommand { get; }
            = new RoutedCommand(nameof(ShowReleaseNotesCommand), typeof(Window));
        public static RoutedCommand CheckForUpdates { get; }
            = new RoutedCommand(nameof(CheckForUpdates), typeof(Window));
        public static RoutedCommand ShowAboutCommand { get; }
            = new RoutedCommand(nameof(ShowAboutCommand), typeof(Window));
    }
}
