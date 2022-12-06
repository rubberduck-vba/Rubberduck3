using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Folding;
using Rubberduck.UI.Abstract;
using Rubberduck.UI.RubberduckEditor;
using Rubberduck.UI.RubberduckEditor.TextTransform;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
