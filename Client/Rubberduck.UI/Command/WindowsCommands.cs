using System.Windows.Input;
using System.Windows;

namespace Rubberduck.UI.Command
{
    public static class WindowsCommands
    {
        public static RoutedCommand NewHorizontalDocumentGroup { get; }
            = new RoutedCommand(nameof(NewHorizontalDocumentGroup), typeof(Window));
        public static RoutedCommand NewVerticalDocumentGroup { get; }
            = new RoutedCommand(nameof(NewVerticalDocumentGroup), typeof(Window));
        public static RoutedCommand CloseAllTabsCommand { get; } = FileCommands.CloseAllDocumentsCommand;
        // TODO PinTab, CloseAllButPinned, etc.

        public static RoutedCommand ShowDocumentWindow { get; }
            = new RoutedCommand(nameof(ShowDocumentWindow), typeof(Window));
    }
}
