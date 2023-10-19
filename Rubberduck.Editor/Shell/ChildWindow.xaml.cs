﻿using System.Windows;

namespace Rubberduck.Editor.Shell
{
    /// <summary>
    /// Interaction logic for ChildWindow.xaml
    /// </summary>
    public partial class ChildWindow : Window
    {
        public ChildWindow(ChildWindowViewModel vm) : this()
        {
            DataContext = vm;
        }

        public ChildWindow()
        {
            InitializeComponent();
        }
    }
}