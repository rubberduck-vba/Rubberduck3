using Ookii.Dialogs.Wpf;
using System.Windows.Input;
using System.Windows;
using System;
using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model;
using System.Threading.Tasks;
using Rubberduck.UI.Services;

namespace Rubberduck.UI.Command
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
                RootFolder = Environment.SpecialFolder.MyDocuments,
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

    public class ShowRubberduckSettingsCommand : CommandBase
    {
        public ShowRubberduckSettingsCommand(ServiceHelper service) 
            : base(service)
        {
        }

        protected async override Task OnExecuteAsync(object? parameter)
        {
            throw new NotImplementedException();
        }
    }

    public class ShowLanguageClientSettingsCommand : CommandBase
    {
        public ShowLanguageClientSettingsCommand(ServiceHelper service)
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
        public ShowLanguageServerSettingsCommand(ServiceHelper service)
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
        public ShowEditorSettingsCommand(ServiceHelper service)
            : base(service)
        {
        }

        protected async override Task OnExecuteAsync(object? parameter)
        {
            throw new NotImplementedException();
        }
    }
}
