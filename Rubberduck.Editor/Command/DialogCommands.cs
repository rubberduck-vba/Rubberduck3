using Rubberduck.Editor.Message;
using Ookii.Dialogs.Wpf;
using System.Windows.Input;
using System.Windows;
using System;

namespace Rubberduck.Editor.Command
{
    public interface IBrowseLocationModel
    {
        string Root { get; }
        string Title { get; }
        string Location { get; set; }
    }

    public static class DialogCommands
    {
        public static RoutedCommand BrowseLocationCommand { get; }
            = new RoutedCommand(nameof(BrowseLocationCommand), null);

        public static void BrowseLocation(Window owner)
        {
            var model = ((IBrowseLocationModel)owner.DataContext)
                ?? throw new ArgumentException("Owner Window.DataContext cannot be null.");
            
            var dialog = new VistaFolderBrowserDialog
            {
                Multiselect = false,
                RootFolder = System.Environment.SpecialFolder.MyDocuments,
                UseDescriptionForTitle = true,
                Description = model.Title,
                ShowNewFolderButton = true,
            };
            if (dialog.ShowDialog(owner) ?? false)
            {
                model.Location = dialog.SelectedPath;
            }
        }
    }
}
