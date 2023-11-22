using Rubberduck.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Rubberduck.UI.Shell
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

        public bool ShowGearButton => ShowSettingsCommand is not null;
        public ICommand? ShowSettingsCommand { get; set; }
        public static readonly DependencyProperty ShowSettingsCommandProperty =
            DependencyProperty.Register(nameof(ShowSettingsCommand), typeof(ICommand), typeof(ThunderFrame));

        public object? ShowSettingsCommandParameter { get; set; }
        public static readonly DependencyProperty ShowSettingsCommandParameterProperty =
            DependencyProperty.Register(nameof(ShowSettingsCommandParameter), typeof(object), typeof(ThunderFrame));

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
