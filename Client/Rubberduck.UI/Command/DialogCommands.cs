using Ookii.Dialogs.Wpf;
using System.Windows.Input;
using System;
using System.Threading.Tasks;
using Rubberduck.UI.Services;
using System.Windows.Controls;
using Rubberduck.UI.Settings;

namespace Rubberduck.UI.Command
{
    public interface IBrowseSelectionModel
    {
        Uri RootUri { get; set; }
        string Title { get; set; }
        string Selection { get; set; }
    }

    public interface IBrowseFolderModel : IBrowseSelectionModel
    {
    }

    public interface IBrowseFileModel : IBrowseSelectionModel
    {
        string DefaultFileExtension { get; set; }
        string Filter { get; set; }
    }

    public record class BrowseFileModel : IBrowseFileModel
    {
        public string DefaultFileExtension { get; set; }
        public string Filter { get; set; }
        public Uri RootUri { get; set; }
        public string Title { get; set; }
        public string Selection { get; set; }
    }

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

    public class ShowRubberduckSettingsCommand : CommandBase
    {
        private readonly ISettingsDialogService _settingsDialog;

        public ShowRubberduckSettingsCommand(UIServiceHelper service, ISettingsDialogService settingsDialog) 
            : base(service)
        {
            _settingsDialog = settingsDialog;
        }

        protected async override Task OnExecuteAsync(object? parameter)
        {
            if (parameter is string key)
            {
                _settingsDialog.ShowDialog(key);
            }
            else
            {
                _settingsDialog.ShowDialog();
            }
        }
    }

    public class ShowLanguageClientSettingsCommand : CommandBase
    {
        public ShowLanguageClientSettingsCommand(UIServiceHelper service)
            : base(service)
        {
        }

        protected async override Task OnExecuteAsync(object? parameter)
        {
            throw new NotImplementedException();
        }
    }

    public class ShowLanguageServerSettingsCommand : CommandBase
    {
        public ShowLanguageServerSettingsCommand(UIServiceHelper service)
            : base(service)
        {
        }

        protected async override Task OnExecuteAsync(object? parameter)
        {
            throw new NotImplementedException();
        }
    }

    public class ShowEditorSettingsCommand : CommandBase
    {
        public ShowEditorSettingsCommand(UIServiceHelper service)
            : base(service)
        {
        }

        protected async override Task OnExecuteAsync(object? parameter)
        {
            throw new NotImplementedException();
        }
    }
}
