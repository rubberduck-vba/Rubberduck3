using System.Windows.Input;

namespace Rubberduck.UI.Shared.Message
{
    /// <summary>
    /// Interaction logic for MessageWindow.xaml
    /// </summary>
    public partial class MessageWindow : System.Windows.Window
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
