using Rubberduck.UI.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace Rubberduck.UI.WinForms
{
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
