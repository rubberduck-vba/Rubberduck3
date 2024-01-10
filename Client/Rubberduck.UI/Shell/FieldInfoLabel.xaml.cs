using System.Windows;
using System.Windows.Controls;

namespace Rubberduck.UI.Shell
{
    /// <summary>
    /// Interaction logic for FieldInfoLabel.xaml
    /// </summary>
    public partial class FieldInfoLabel : UserControl
    {
        public string Text 
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(FieldInfoLabel));

        public FieldInfoLabel()
        {
            InitializeComponent();
        }
    }
}
