﻿using System.Windows.Input;

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
            MouseDown += OnMouseDown;
        }

        public MessageWindow()
        {
            InitializeComponent();
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }
    }
}
