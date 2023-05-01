using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Rubberduck.UI.Abstract
{
    public interface IDialogShellViewModel
    {
        string TitleText { get; set; }
        BitmapImage IconSource { get; set; }
        string TitleLabelText { get; set; }
        string InstructionsLabelText { get; set; }
        string ContentsText { get; set; }
        UserControl MainContentControl { get; set; }
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
