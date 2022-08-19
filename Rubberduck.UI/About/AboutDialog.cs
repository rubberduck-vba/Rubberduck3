using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rubberduck.UI.About
{

    public partial class AboutDialog : Form
    {
        public AboutDialog()
        {
            InitializeComponent();
        }

        public AboutDialog(IAboutControlViewModel viewModel) : this()
        {
            ViewModel = viewModel;
        }

        private IAboutControlViewModel ViewModel { get; }

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
