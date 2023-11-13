using Ookii.Dialogs.Wpf;
using System.Windows.Input;
using System;
using System.Threading.Tasks;
using Rubberduck.UI.Services;
using System.Windows.Controls;

namespace Rubberduck.UI.Command
{
    public static class DialogCommands
    {
        public static RoutedCommand BrowseLocationCommand { get; }
            = new RoutedCommand(nameof(BrowseLocationCommand), typeof(TextBox));

        public static void BrowseLocation(TextBox owner)
        {
            var dialog = new VistaFolderBrowserDialog
            {
                Multiselect = false,
                RootFolder = Environment.SpecialFolder.MyDocuments,
                UseDescriptionForTitle = true,
                ShowNewFolderButton = true,
            };
            if (dialog.ShowDialog() ?? false)
            {
                owner.Text = dialog.SelectedPath;
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
