using Rubberduck.Resources;
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

namespace Rubberduck.Editor.Shell
{
    /// <summary>
    /// Interaction logic for ThunderFrame.xaml
    /// </summary>
    public partial class ThunderFrame : UserControl
    {
        public ThunderFrame()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The content of the title label.
        /// </summary>
        public string Title { get; set; } = RubberduckUI.Rubberduck;
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(ThunderFrame));

        public ICommand? AcceptCommand { get; set; }
        public static readonly DependencyProperty AcceptCommandProperty =
            DependencyProperty.Register(nameof(AcceptCommand), typeof(ICommand), typeof(ThunderFrame));
        public ICommand? CancelCommand { get; set; }
        public static readonly DependencyProperty CancelCommandProperty =
            DependencyProperty.Register(nameof(CancelCommand), typeof(ICommand), typeof(ThunderFrame));

        public string AcceptButtonText { get; set; } = RubberduckUI.OK;
        public static readonly DependencyProperty AcceptButtonTextProperty =
            DependencyProperty.Register(nameof(AcceptButtonText), typeof(bool), typeof(ThunderFrame));

        public string CancelButtonText { get; set; } = RubberduckUI.CancelButtonText;
        public static readonly DependencyProperty CancelButtonTextProperty =
            DependencyProperty.Register(nameof(CancelButtonText), typeof(bool), typeof(ThunderFrame));

        public bool ShowAcceptButton => AcceptCommand is not null;
        public static readonly DependencyProperty ShowAcceptButtonProperty =
            DependencyProperty.Register(nameof(ShowAcceptButton), typeof(bool), typeof(ThunderFrame));

        public bool ShowCancelButton => CancelCommand is not null;
        public static readonly DependencyProperty ShowCancelButtonProperty =
            DependencyProperty.Register(nameof(ShowCancelButton), typeof(bool), typeof(ThunderFrame));
    }
}
