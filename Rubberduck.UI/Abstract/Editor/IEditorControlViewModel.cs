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
        ObservableCollection<ICodePaneViewModel> ModuleDocumentTabs { get; }
        ICodePaneViewModel SelectedModuleDocumentTab { get; set; }
        IEnumerable<IShellToolTab> ToolTabs { get; }
        IShellToolTab SelectedToolTab { get; set; }
        IStatusBarViewModel Status { get; }
        IEnumerable<ISyntaxErrorViewModel> SyntaxErrors { get; }

        ICodePaneViewModel GetModule(QualifiedModuleName module);
        bool LoadModule(QualifiedModuleName module, string content, IMemberProviderViewModel vm);
        bool UnloadModule(QualifiedModuleName module);
        void ActivateModuleDocumentTab(QualifiedModuleName module);
    }
}
