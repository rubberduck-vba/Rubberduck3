using Rubberduck.Resources.Messages;
using System.Windows.Input;

namespace Rubberduck.UI.Windows
{
    public class WindowViewModel : ViewModelBase
    {
        public string Title { get; init; } = "Title";

        public bool ShowGearButton => ShowSettingsCommand != null;
        public ICommand? ShowSettingsCommand { get; init; }

        public virtual bool ShowAcceptButton { get; } = true;
        public virtual bool ShowCancelButton { get; } = true;

        public virtual string AcceptButtonText { get; } = RubberduckMessages.MessageActionButton_Accept;
        public virtual string CancelButtonText { get; } = RubberduckMessages.MessageActionButton_Cancel;
    }
}
