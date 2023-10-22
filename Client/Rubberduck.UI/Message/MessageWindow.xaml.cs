using System.Windows;

namespace Rubberduck.UI.Message
{
    /// <summary>
    /// Interaction logic for MessageWindow.xaml
    /// </summary>
    public partial class MessageWindow : Window
    {
        public MessageWindow(IMessageWindowViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }

        public MessageWindow()
        {
            InitializeComponent();
        }
    }
}
