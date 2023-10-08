using Rubberduck.UI.Abstract;
using System.Drawing;
using System.Windows.Forms;

namespace Rubberduck.UI.WinForms
// note: not under Rubberduck.UI.WinForms only because this dialog isn't meant to be used anywhere other than for startup.
{
    public partial class Splash : Form
    {
        public Splash()
        {
            InitializeComponent();
        }

        public Splash(ISplashViewModel viewModel)
            : this()
        {
            viewModel.PropertyChanged += ViewModel_PropertyChanged;
            InitializeVersionLabel(viewModel);
            InitializeStatusLabel(viewModel);
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ISplashViewModel.CurrentStatus))
            {
                StatusLabel.Text = ((ISplashViewModel)sender).CurrentStatus;
                StatusLabel.Refresh();
            }
        }

        private void InitializeVersionLabel(ISplashViewModel viewModel)
        {
            VersionLabel.DataBindings.Add(new Binding(nameof(VersionLabel.Text), viewModel, nameof(viewModel.Version)));

            VersionLabel.BackColor = Color.Transparent;
            VersionLabel.Parent = ContainerBox;
        }

        private void InitializeStatusLabel(ISplashViewModel viewModel)
        {
            StatusLabel.DataBindings.Add(new Binding(nameof(StatusLabel.Text), viewModel, nameof(viewModel.CurrentStatus)));

            StatusLabel.BackColor = Color.Transparent;
            StatusLabel.Parent = ContainerBox;
        }
    }
}
