using Rubberduck.Resources.v3;
using Rubberduck.UI.Command.Abstract;
using Rubberduck.UI.Services;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Rubberduck.UI.Shell
{
    /// <summary>
    /// Interaction logic for SearchBox.xaml
    /// </summary>
    public partial class SearchBox : UserControl
    {
        public SearchBox(/*designer*/)
        {
            Text = string.Empty;
            Hint = RubberduckUICommands.ResourceManager.GetString(nameof(RubberduckUICommands.SearchBox_DefaultHint)) ?? $"[missing key:{nameof(RubberduckUICommands.SearchBox_DefaultHint)}]";
            InitializeComponent();
            ValueContainerInput = ValueContainer;
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(SearchBox), new UIPropertyMetadata(default(string), PropertyChangedCallback));
        public static readonly DependencyProperty HintProperty =
            DependencyProperty.Register(nameof(Hint), typeof(string), typeof(SearchBox), new UIPropertyMetadata(default(string), PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            if (source is SearchBox control)
            {
                var newValue = (string)e.NewValue;
                switch (e.Property.Name)
                {
                    case nameof(Text):
                        control.Text = newValue;
                        break;
                    case nameof(Hint):
                        control.Hint = newValue;
                        break;
                }
            }
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set
            {
                var old = GetValue(TextProperty);
                SetValue(TextProperty, value);
                OnPropertyChanged(new DependencyPropertyChangedEventArgs(TextProperty, old, value));
            }
        }
        public string Hint
        {
            get => (string)GetValue(HintProperty);
            set
            {
                var old = GetValue(HintProperty);
                SetValue(HintProperty, value);
                OnPropertyChanged(new DependencyPropertyChangedEventArgs(HintProperty, old, value));
            }
        }

        public event EventHandler TextChanged = delegate { };

        public ICommand ClearSearchCommand => new DelegateCommand(UIServiceHelper.Instance!, (arg) => Text = string.Empty) { Name = nameof(ClearSearchCommand) };

        public TextBox ValueContainerInput { get; }
    }
}
