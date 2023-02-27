﻿using Rubberduck.UI.Abstract;
using System.Windows;
using System.Windows.Forms;

namespace Rubberduck.UI.WinForms
{
    public interface IEditorShellWindowProvider
    {
        EditorShellWindow Create();
    }

    public class EditorShellWindowProvider : IEditorShellWindowProvider
    {
        private readonly IEditorShellViewModel _viewModel;

        public EditorShellWindowProvider(IEditorShellViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public EditorShellWindow Create()
        {
            return new EditorShellWindow(_viewModel);
        }
    }


    public partial class EditorShellWindow : UserControl, IDockableUserControl
    {
        public EditorShellWindow()
        {
            InitializeComponent();
        }

        public EditorShellWindow(IEditorShellViewModel viewModel)
            : this()
        {
            if (EditorShellHost.Child is FrameworkElement element)
            {
                element.DataContext = viewModel;
            }
        }

        public string ClassId => "E27389E4-01D1-4953-9E15-F61B3AB430C8";

        public string Caption => "Rubberduck Editor";
    }
}
