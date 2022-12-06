using Rubberduck.Parsing;
using Rubberduck.VBEditor;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubberduck.UI.Abstract
{
    public interface IEditorShellViewModel : INotifyPropertyChanged, ITextDocumentProvider
    {
        /// <summary>
        /// An observable collection of loaded modules.
        /// </summary>
        ObservableCollection<ICodePaneViewModel> LoadedModules { get; }

        /// <summary>
        /// An observable collection of tool tabs.
        /// </summary>
        ObservableCollection<IShellToolTab> ToolTabs { get; }

        /// <summary>
        /// The editor shell status bar view model.
        /// </summary>
        IStatusBarViewModel Status { get; }
    }
}
