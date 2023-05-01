using Rubberduck.UI.Abstract;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

///<summary>
///This file provides design-time data for the <see cref="DialogShell.xaml"/> control.
///This is only to support working in the XAML designer and nothing in this file should be used otherwise.
///</summary>
namespace Rubberduck.UI.Xaml.Controls
{
    internal class DialogShellDesignViewModel : IDialogShellViewModel
    {
        public string TitleText { get; set; } = "Title";
        public BitmapImage IconSource { get; set; } = new BitmapImage(new Uri("pack://application:,,,/Rubberduck.Resources;component/Icons/Custom/PNG/ObjectEvent.png"));
        public string TitleLabelText { get; set; } = "Title label text";
        public string InstructionsLabelText { get; set; } = "Instructions text";
        public string ContentsText { get; set; } = "Contents text";
        public UserControl MainContentControl 
        {
            get
            {
                var control = new UserControl();

                bool showText = true;
                if (showText)
                {
                    var text = new TextBlock { TextWrapping = TextWrapping.Wrap };
                    Binding textBinding = new Binding("ContentsText")
                    {
                        Mode = BindingMode.TwoWay
                    };
                    text.SetBinding(TextBlock.TextProperty, textBinding);
                    Binding minHeightBinding = new Binding("ClientAreaMinHeight")
                    {
                        Mode = BindingMode.TwoWay
                    };
                    text.SetBinding(TextBlock.MinHeightProperty, minHeightBinding);
                    var scrollViewer = new ScrollViewer { Content = text };
                    control.Content = scrollViewer;
                }
                else
                {
                    var inputBox = new TextBox { TextWrapping = TextWrapping.NoWrap, AcceptsReturn = false, Height=20, VerticalAlignment=VerticalAlignment.Top};
                    Binding textBinding = new Binding("ContentsText")
                    {
                        Mode = BindingMode.TwoWay
                    };
                    inputBox.SetBinding(TextBox.TextProperty, textBinding);
                    control.Content = inputBox;
                }
                return control;
            }
            set { }
        }
        public string MoreInformationText { get; set; } = "More Information goes here...";
        public string OptionLabelText { get; set; } = "Option label";
        public bool OptionIsChecked { get; set; } = true;
        public string CancelButtonText { get; set; } = "Cancel";
        public string DefaultButtonText { get; set; } = "Default";
        public bool HasOption { get; set; } = true;
        public bool HasCancelButton { get; set; } = true;
        public bool CanResize { get; set; } = true;
        public int ClientAreaMinHeight { get; set; } = 100;
    }
}
