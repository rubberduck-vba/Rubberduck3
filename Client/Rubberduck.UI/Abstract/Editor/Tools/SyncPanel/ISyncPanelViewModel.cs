using Rubberduck.UI.Abstract.Editor.Tools;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace Rubberduck.UI.Abstract.Editor.Tools.SyncPanel
{
    public interface ISyncPanelViewModel : IToolTabViewModel, INotifyPropertyChanged
    {
        ObservableCollection<ISyncPanelModuleViewModel> VBIDEModules { get; }
        ISyncPanelModuleViewModel SelectedVBIDEModule { get; set; }

        /// <summary>
        /// Gets a command that loads the list of VBIDE modules.
        /// </summary>
        ICommand ReloadCommand { get; }

        /// <summary>
        /// Gets a command that synchronizes all modules between the VBIDE and the editor shell.
        /// </summary>
        ICommand SyncCommand { get; }
    }
}
