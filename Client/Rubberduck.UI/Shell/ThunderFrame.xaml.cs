using Rubberduck.Resources;
using System;
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
            Title = RubberduckUI.Rubberduck;
            ShowAcceptButton = true;
            AcceptButtonText = RubberduckUI.OK;
            CancelButtonText = RubberduckUI.CancelButtonText;

            MouseDown += OnMouseDown;
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                var window = FindWindowParent(this);
                window.DragMove();
            }
        }

        private Window FindWindowParent(DependencyObject obj)
        {
            if (obj is Window window)
            {
                return window;
            }

            var parent = LogicalTreeHelper.GetParent(obj);
            if (parent is not null)
            {
                return FindWindowParent(parent);
            }

            throw new InvalidOperationException();
        }

        /// <summary>
        /// The content of the title label.
        /// </summary>
        public string Title 
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(ThunderFrame));

        public bool ShowGearButton => ShowSettingsCommand is not null;
        public ICommand? ShowSettingsCommand
        { 
            get => (ICommand)GetValue(ShowSettingsCommandProperty);
            set => SetValue(ShowSettingsCommandProperty, value);
        }
        public static readonly DependencyProperty ShowSettingsCommandProperty =
            DependencyProperty.Register(nameof(ShowSettingsCommand), typeof(ICommand), typeof(ThunderFrame));

        public object? ShowSettingsCommandParameter 
        {
            get => GetValue(ShowSettingsCommandParameterProperty);
            set => SetValue(ShowSettingsCommandParameterProperty, value);
        }
        public static readonly DependencyProperty ShowSettingsCommandParameterProperty =
            DependencyProperty.Register(nameof(ShowSettingsCommandParameter), typeof(object), typeof(ThunderFrame));

        public ICommand? AcceptCommand 
        {
            get => (ICommand)GetValue(AcceptCommandProperty);
            set => SetValue(AcceptCommandProperty, value);
        }
        public static readonly DependencyProperty AcceptCommandProperty =
            DependencyProperty.Register(nameof(AcceptCommand), typeof(ICommand), typeof(ThunderFrame));
        public ICommand? CancelCommand 
        {
            get => (ICommand)GetValue(CancelCommandProperty);
            set => SetValue(CancelCommandProperty, value);
        }
        public static readonly DependencyProperty CancelCommandProperty =
            DependencyProperty.Register(nameof(CancelCommand), typeof(ICommand), typeof(ThunderFrame));

        public string AcceptButtonText 
        { 
            get => (string)GetValue(AcceptButtonTextProperty);
            set => SetValue(AcceptButtonTextProperty, value);
        } 
        public static readonly DependencyProperty AcceptButtonTextProperty =
            DependencyProperty.Register(nameof(AcceptButtonText), typeof(string), typeof(ThunderFrame));

        public string CancelButtonText 
        {
            get => (string)GetValue(CancelButtonTextProperty);
            set => SetValue(CancelButtonTextProperty, value);
        }
        public static readonly DependencyProperty CancelButtonTextProperty =
            DependencyProperty.Register(nameof(CancelButtonText), typeof(string), typeof(ThunderFrame));

        public bool ShowAcceptButton 
        { 
            get => (bool)GetValue(ShowAcceptButtonProperty);
            set => SetValue(ShowAcceptButtonProperty, value);
        }
        public static readonly DependencyProperty ShowAcceptButtonProperty =
            DependencyProperty.Register(nameof(ShowAcceptButton), typeof(bool), typeof(ThunderFrame));

        public bool ShowCancelButton
        {
            get => (bool)GetValue(ShowCancelButtonProperty);
            set => SetValue(ShowCancelButtonProperty, value);
        }
        public static readonly DependencyProperty ShowCancelButtonProperty =
            DependencyProperty.Register(nameof(ShowCancelButton), typeof(bool), typeof(ThunderFrame));

        public bool ShowTitle
        {
            get => (bool)GetValue(ShowTitleProperty);
            set => SetValue(ShowTitleProperty, value);
        }
        public static readonly DependencyProperty ShowTitleProperty =
            DependencyProperty.Register(nameof(ShowTitle), typeof(bool), typeof(ThunderFrame));
    }
}
