using Rubberduck.UI.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Rubberduck.UI.Xaml.Controls
{
    /// <summary>
    /// Interaction logic for DialogShell.xaml
    /// </summary>
    public partial class DialogShell : UserControl
    {
        public DialogShell(IDialogShellViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }

        private IDialogShellViewModel ViewModel => DataContext as IDialogShellViewModel;


        private void HandleOptionChecked(object sender, RoutedEventArgs e)
        {
            ViewModel.OptionIsChecked = true;
        }

        private void HandleOptionUnchecked(object sender, RoutedEventArgs e)
        {
            ViewModel.OptionIsChecked = false;
        }
    }
}
