using Ookii.Dialogs.Wpf;
using System.Windows;
using System.Windows.Controls;

namespace Rubberduck.UI.NewProject
{
    /// <summary>
    /// Interaction logic for NewProjectWindow.xaml
    /// </summary>
    public partial class NewProjectWindow : Window
    {
        public NewProjectWindow(NewProjectWindowViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }

        public NewProjectWindow()
        {
            InitializeComponent();
        }

        private NewProjectWindowViewModel ViewModel => (NewProjectWindowViewModel)DataContext;

        private void BrowseCommandBinding_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            var textBox = e.Parameter as TextBox;
            if (VistaFolderBrowserDialog.IsVistaFolderDialogSupported)
            {
                var dialog = new VistaFolderBrowserDialog();
                // configure?
                if (dialog.ShowDialog() == true)
                {
                    if (textBox != null)
                    {
                        textBox.Text = dialog.SelectedPath;
                    }
                    else
                    {
                        ViewModel.WorkspaceLocation = dialog.SelectedPath;
                    }
                }
            }
        }
    }
}
