using Ookii.Dialogs.Wpf;
using System.Windows.Input;
using System;
using System.Windows.Controls;

namespace Rubberduck.UI.Command
{
    public static class DialogCommands
    {
        public static RoutedCommand BrowseLocationCommand { get; }
            = new RoutedCommand(nameof(BrowseLocationCommand), typeof(TextBox));

        public static bool BrowseLocation(IBrowseFolderModel model)
        {
            var dialog = new VistaFolderBrowserDialog
            {
                SelectedPath = model.Selection,
                Description = model.Title,
                RootFolder = Environment.SpecialFolder.LocalApplicationData,
                Multiselect = false,
                UseDescriptionForTitle = true,
                ShowNewFolderButton = true,
            };

            var didAccept = dialog.ShowDialog() == true;
            if (didAccept)
            {
                model.Selection = dialog.SelectedPath;
            }

            return didAccept;
        }

        public static bool BrowseFileOpen(IBrowseFileModel model)
        {
            var dialog = new VistaOpenFileDialog
            {
                FileName = model.Selection,
                InitialDirectory = model.RootUri.LocalPath,
                Title = model.Title,
                Filter = model.Filter,
                DefaultExt = model.DefaultFileExtension,
                Multiselect = false,
                AddExtension = true,
                CheckPathExists = true,
                CheckFileExists = true,
                DereferenceLinks = true,
                RestoreDirectory = true,
            };

            var didAccept = dialog.ShowDialog() == true;
            if (didAccept)
            {
                model.Selection = dialog.FileName;
            }

            return didAccept;
        }

        public static bool BrowseFileSaveAs(IBrowseFileModel model)
        {
            var dialog = new VistaSaveFileDialog
            {
                FileName = model.Selection,
                InitialDirectory = model.RootUri.LocalPath,
                Title = model.Title,
                Filter = model.Filter,
                DefaultExt = model.DefaultFileExtension,
                AddExtension = true,
                ValidateNames = true,
                CheckPathExists = true,
                OverwritePrompt = true,
                DereferenceLinks = true,
                RestoreDirectory = true,
            };

            var didAccept = dialog.ShowDialog() == true;
            if (didAccept)
            {
                model.Selection = dialog.FileName;
            }

            return didAccept;
        }
    }
}
