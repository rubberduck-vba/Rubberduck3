using Rubberduck.UI.Abstract;
using System.Windows.Controls;

namespace Rubberduck.UI.Xaml.Controls
{


    /// <summary>
    /// Interaction logic for EditorControl.xaml
    /// </summary>
    public partial class EditorShellControl : UserControl
    {
        public EditorShellControl()
        {
            InitializeComponent();
        }

        private void FilterLeftPanelItems(object sender, System.Windows.Data.FilterEventArgs e)
        {
            e.Accepted = (e.Item as IShellToolTab)?.Settings.TabPanelLocation == ToolTabLocation.LeftPanel;
        }

        private void FilterRightPanelItems(object sender, System.Windows.Data.FilterEventArgs e)
        {
            e.Accepted = (e.Item as IShellToolTab)?.Settings.TabPanelLocation == ToolTabLocation.RightPanel;
        }

        private void FilterBottomPanelItems(object sender, System.Windows.Data.FilterEventArgs e)
        {
            e.Accepted = (e.Item as IShellToolTab)?.Settings.TabPanelLocation == ToolTabLocation.BottomPanel;
        }
    }
}
