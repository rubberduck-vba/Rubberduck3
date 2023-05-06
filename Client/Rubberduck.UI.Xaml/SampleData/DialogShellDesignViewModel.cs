using Rubberduck.UI.Abstract;
using System;
using System.Windows.Media.Imaging;
using System.Windows;
using Rubberduck.UI.WPF.Converters;

///<summary>
///This file provides design-time data for the <see cref="DialogShell.xaml"/> control.
///This is only to support working in the XAML designer and nothing in this file should be used otherwise.
///</summary>
namespace Rubberduck.UI.Xaml.Controls
{
    internal class DialogShellDesignViewModel : IDialogShellViewModel
    {
        public DialogShellDesignViewModel() 
        {
            TitleText = "Title";
            IconSource = DialogType.Information;
            TitleLabelText = "Title label text";
            InstructionsLabelText = "Instructions text";
            ContentsText = "Contents text";
            MainContentIsInput = false;
            MoreInformationText = "More Information goes here...";
            OptionLabelText = "Option label";
            OptionIsChecked = true;
            CancelButtonText = "Cancel";
            DefaultButtonText = "Default";
            HasOption = true;
            HasCancelButton = true;
            CanResize = true;
            ClientAreaMinHeight = 100;
        }

        public string TitleText { get; set; }
        public DialogType IconSource { get; set; }
        public string TitleLabelText { get; set; }
        public string InstructionsLabelText { get; set; }
        public string ContentsText { get; set; }
        public bool MainContentIsInput { get; set; }
        public string MoreInformationText { get; set; }
        public string OptionLabelText { get; set; }
        public bool OptionIsChecked { get; set; }
        public string CancelButtonText { get; set; }
        public string DefaultButtonText { get; set; }
        public bool HasOption { get; set; }
        public bool HasCancelButton { get; set; }
        public bool CanResize { get; set; }
        public int ClientAreaMinHeight { get; set; }
    }
}
