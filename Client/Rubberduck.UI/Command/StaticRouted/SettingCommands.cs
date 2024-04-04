using System.Windows.Input;
using System.Windows.Controls;

namespace Rubberduck.UI.Command.StaticRouted;

public static class SettingCommands
{
    public static RoutedCommand ShowSettingsCommand { get; }
        = new RoutedCommand(nameof(ShowSettingsCommand), typeof(UserControl));

    public static RoutedCommand AddListSettingItemCommand { get; }
        = new RoutedCommand(nameof(AddListSettingItemCommand), typeof(UserControl));
}