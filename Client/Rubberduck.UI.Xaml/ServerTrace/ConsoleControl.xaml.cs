using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Rubberduck.UI.Xaml.ServerTrace
{
    /// <summary>
    /// Interaction logic for ConsoleControl.xaml
    /// </summary>
    public partial class ConsoleControl : UserControl
    {
        public ConsoleControl()
        {
            InitializeComponent();
            DataContextChanged += ConsoleControl_DataContextChanged;
        }

        private void ConsoleControl_DataContextChanged(object? sender, DependencyPropertyChangedEventArgs e)
        {
            if (ViewModel is IConsoleViewModel vm)
            {
                vm.PropertyChanged += ViewModel_PropertyChanged;
            }
        }

        private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IConsoleViewModel.ConsoleContent))
            {
                var lastItem = ViewModel.ConsoleContent.Last();
                //ContentArea.ScrollIntoView(lastItem);
            }
        }

        private IConsoleViewModel ViewModel => (IConsoleViewModel)DataContext;
    }
}
