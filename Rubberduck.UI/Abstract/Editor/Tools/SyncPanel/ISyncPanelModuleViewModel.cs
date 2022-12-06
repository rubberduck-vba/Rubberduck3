using Rubberduck.Parsing.Model;
using Rubberduck.VBEditor;
using System.ComponentModel;
using System.Windows.Input;

namespace Rubberduck.UI.Abstract
{
    public interface ISyncPanelModuleViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets/sets an object representing the VBIDE module.
        /// </summary>
        QualifiedModuleName QualifiedModuleName { get; set; }

        /// <summary>
        /// Gets/sets the ModuleType of this module.
        /// </summary>
        ModuleType ModuleType { get; set; }

        /// <summary>
        /// Gets/sets the synchronization status of this module.
        /// </summary>
        ModuleSyncState State { get; set; }

        /// <summary>
        /// Gets a command that synchronizes this module between the VBIDE and the editor shell.
        /// </summary>
        ICommand SyncCommand { get; }

        /// <summary>
        /// Gets a command that load this module from the VBIDE into the editor shell.
        /// </summary>
        ICommand LoadCommand { get; }
    }
}
