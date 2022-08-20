using Rubberduck.UI.Abstract;
using Rubberduck.UI.Xaml.Controls;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace Rubberduck.UI.WinForms.Dialogs
{
    public partial class AboutDialog : Form
    {
        public AboutDialog()
        {
            InitializeComponent();
        }

        private ElementHost AboutControlHost { get; }

        public AboutDialog(IAboutControlViewModel viewModel) : this()
        {
            var ui = new AboutControl { DataContext = viewModel };

            // this sucks, but we need to be doing this at run-time because the designer refuses to create the WPF UserControl.
            // error says there is no default constructor for some reason. there IS a default constructor... it's all there is.
            AboutControlHost = new ElementHost { Child = ui };

            Controls.Add(AboutControlHost);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
