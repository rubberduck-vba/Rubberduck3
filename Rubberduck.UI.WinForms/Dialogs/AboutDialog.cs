using Rubberduck.UI.Abstract;
using System.Windows.Forms;

namespace Rubberduck.UI.WinForms.Dialogs
{
    public partial class AboutDialog : Form
    {
        public AboutDialog()
        {
            InitializeComponent();
        }

        public AboutDialog(IAboutControlViewModel viewModel) : this()
        {
            //AboutControl1.DataContext = viewModel;
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
