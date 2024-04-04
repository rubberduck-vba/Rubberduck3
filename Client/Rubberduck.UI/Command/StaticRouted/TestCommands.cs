using System.Windows.Input;
using System.Windows;

namespace Rubberduck.UI.Command.StaticRouted
{
    public static class TestCommands
    {
        public static RoutedCommand RunAllTestsCommand { get; }
            = new RoutedCommand(nameof(RunAllTestsCommand), typeof(Window));
        public static RoutedCommand RepeatLastRunCommand { get; }
            = new RoutedCommand(nameof(RepeatLastRunCommand), typeof(Window));
        public static RoutedCommand ShowTestExplorerCommand { get; } = ViewCommands.ShowTestExplorerCommand;
    }
}
