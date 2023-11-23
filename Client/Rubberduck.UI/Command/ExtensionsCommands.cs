using System.Windows.Input;
using System.Windows;

namespace Rubberduck.UI.Command
{
    public static class ExtensionsCommands
    {
        public static RoutedCommand ManageExtensionsCommand { get; }
            = new RoutedCommand(nameof(ManageExtensionsCommand), typeof(Window));
    }
}
