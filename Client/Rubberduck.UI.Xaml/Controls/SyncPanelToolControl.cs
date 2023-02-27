using Rubberduck.UI.Abstract;
using System.Windows;
using System.Windows.Controls;

namespace Rubberduck.UI.Xaml.Controls
{
    /// <summary>
    /// Interaction logic for SyncStatusToolControl.xaml
    /// </summary>
    public partial class SyncPanelToolControl : UserControl
    {
        public SyncPanelToolControl()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private ISyncPanelViewModel ViewModel => DataContext as ISyncPanelViewModel;

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ViewModel?.ReloadCommand?.Execute(null);
        }
    }
}
