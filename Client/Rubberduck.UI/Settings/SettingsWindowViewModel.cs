using Rubberduck.Resources;
using Rubberduck.UI.Command;
using System;
using System.Windows.Input;

namespace Rubberduck.UI.Settings
{
    public interface ISettingsWindowViewModel { }

    public class SettingsWindowViewModel : DialogWindowViewModel, ISettingsWindowViewModel
    {
        public SettingsWindowViewModel(MessageActionCommand[] actions, ICommand? showSettingsCommand = null) 
            : base(RubberduckUI.Settings, actions, showSettingsCommand)
        {
        }

        protected override void ResetToDefaults()
        {
            throw new NotImplementedException();
        }
    }
}
