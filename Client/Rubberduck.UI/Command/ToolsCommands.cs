﻿using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;

namespace Rubberduck.UI.Command
{
    public static class ToolsCommands
    {
        public static RoutedCommand CloseToolwindowCommand { get; }
            = new RoutedCommand(nameof(CloseToolwindowCommand), typeof(UserControl));

        public static RoutedCommand ShowRegularExpressionToolCommand { get; }
            = new RoutedCommand(nameof(ShowRegularExpressionToolCommand), typeof(Window));
        public static RoutedCommand ShowRubberduckSettingsCommand { get; }
            = new RoutedCommand(nameof(ShowRubberduckSettingsCommand), typeof(Window));
    }
}
