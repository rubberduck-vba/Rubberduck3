using System.Drawing;
using System.Windows.Forms;

namespace Rubberduck.UI.Splash
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
            BindVersionLabel(viewModel);
            BindStatusLabel(viewModel);
        }

        private void BindVersionLabel(ISplashViewModel viewModel)
        {
            VersionLabel.DataBindings.Add(new Binding(nameof(viewModel.Version), viewModel, nameof(VersionLabel.Text)));
            
            VersionLabel.BackColor = Color.Transparent;
            VersionLabel.Parent = ContainerBox;
        }

        private void BindStatusLabel(ISplashViewModel viewModel)
        {
            StatusLabel.DataBindings.Add(new Binding(nameof(viewModel.InitializationStatus), viewModel, nameof(StatusLabel.Text), false, DataSourceUpdateMode.OnPropertyChanged));
            
            StatusLabel.BackColor = Color.Transparent;
            StatusLabel.Parent = ContainerBox;
        }
    }
}
