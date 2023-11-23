using System.Windows.Input;
using System.Windows;

namespace Rubberduck.UI.Command
{
    public static class DebugCommands
    {
        public static RoutedCommand RunCommand { get; }
            = new RoutedCommand(nameof(RunCommand), typeof(Window));
    }
}
