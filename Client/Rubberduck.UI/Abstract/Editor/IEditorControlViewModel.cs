using Rubberduck.InternalApi.Model.Abstract;
using Rubberduck.UI.Abstract.Editor.Tools;
using Rubberduck.Unmanaged.Model.Abstract;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Rubberduck.UI.Abstract.Editor
{

    public interface IEditorShellViewModel : INotifyPropertyChanged, ITextDocumentProvider
    {
        ObservableCollection<ICodePaneViewModel> ModuleDocumentTabs { get; }
        ICodePaneViewModel SelectedModuleDocumentTab { get; set; }
        IEnumerable<IShellToolTab> ToolTabs { get; }
        IShellToolTab SelectedToolTab { get; set; }
        IStatusBarViewModel Status { get; }
        IEnumerable<ISyntaxErrorViewModel> SyntaxErrors { get; }

        ICodePaneViewModel GetModule(IQualifiedModuleName module);
        bool LoadModule(IQualifiedModuleName module, string content, IMemberProviderViewModel vm);
        bool UnloadModule(IQualifiedModuleName module);
        void ActivateModuleDocumentTab(IQualifiedModuleName module);


        public object Partition { get; init; }
        public object InterTabClient { get; init; }
    }
}
