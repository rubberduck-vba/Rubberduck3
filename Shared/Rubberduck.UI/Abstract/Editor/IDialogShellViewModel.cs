using Rubberduck.UI.WPF.Converters;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Rubberduck.UI.Abstract
{
    public interface IDialogShellViewModel
    {
        string TitleText { get; set; }
        DialogType IconSource { get; set; }
        string TitleLabelText { get; set; }
        string InstructionsLabelText { get; set; }
        string ContentsText { get; set; }
        bool MainContentIsInput { get; set; }
        string MoreInformationText { get; set; }
        string OptionLabelText { get; set; }
        bool OptionIsChecked { get; set; }
        string CancelButtonText { get; set; }
        string DefaultButtonText { get; set; }
        bool HasOption { get; set; }
        bool HasCancelButton { get; set; }
        bool CanResize { get; set; }
        int ClientAreaMinHeight { get; set; }
    }
}
